import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { AddressesApi } from '../../services/addresses-api';
import { ServiceAddress } from '../../models/service-address.model';
import { ServiceAddressEntry } from '../../components/service-address-entry/service-address-entry';
import { CommonModule } from '@angular/common';
import { LoggedInLayout } from '../logged-in-layout/logged-in-layout';

@Component({
  selector: 'app-profile-edit',
  imports: [CommonModule, ServiceAddressEntry, LoggedInLayout],
  templateUrl: './profile-edit.html'
})
export class ProfileEdit {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly addressesApi = inject(AddressesApi);

  addresses = signal<ServiceAddress[]>([]);
  loading = signal<boolean>(false);
  editingAddress = signal<ServiceAddress | null>(null);
  showAddForm = signal<boolean>(false);

  ngOnInit() {
    this.loadAddresses();
  }

  loadAddresses() {
    this.loading.set(true);
    this.addressesApi.getAddresses().subscribe({
      next: (addresses) => {
        this.addresses.set(addresses);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load addresses:', err);
        this.loading.set(false);
      }
    });
  }

  onAddressesLoaded(addresses: ServiceAddress[]) {
    this.addresses.set(addresses);
  }

  createNewAddress() {
    this.editingAddress.set(null);
    this.showAddForm.set(true);
  }

  editAddress(address: ServiceAddress) {
    this.editingAddress.set({ ...address });
    this.showAddForm.set(true);
  }

  handleAddressSaved(address: ServiceAddress) {
    const currentAddresses = this.addresses();
    if (this.editingAddress() && address.addressId) {
      const updatedAddresses = currentAddresses.map((a: ServiceAddress) => 
        a.addressId === address.addressId ? address : a
      );
      this.addresses.set(updatedAddresses);
    } else {
      this.addresses.set([...currentAddresses, address]);
    }
    this.showAddForm.set(false);
    this.editingAddress.set(null);
  }

  handleCancel() {
    this.showAddForm.set(false);
    this.editingAddress.set(null);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
