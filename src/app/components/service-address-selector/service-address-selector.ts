import { Component, inject, input, output, signal, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddressesApi } from '../../services/addresses-api';
import { ServiceAddress } from '../../models/service-address.model';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-service-address-selector',
  imports: [CommonModule],
  templateUrl: './service-address-selector.html',
  styleUrls: ['./service-address-selector.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class ServiceAddressSelector implements OnInit {
  private readonly addressesApi = inject(AddressesApi);
  private _loading = signal<boolean>(false);
  private _addresses = signal<ServiceAddress[]>([]);
  private _error = signal<string | null>(null);
  private _selectedAddressId = signal<string>('');
  
  loading = this._loading.asReadonly();
  addresses = this._addresses.asReadonly();
  error = this._error.asReadonly();
  selectedAddressId = input<string>();
  
  addressSelected = output<string>();
  addressesLoaded = output<ServiceAddress[]>();
  
  ngOnInit() {
    this.loadAddresses();
  }
  
  async loadAddresses() {
    this._loading.set(true);
    this._error.set(null);
    
    try {
      const response = await this.addressesApi.getAddresses().pipe(take(1)).toPromise();
      this._addresses.set(response || []);
      this.addressesLoaded.emit(this._addresses());
      
      const selectedId = this.selectedAddressId();
      const addresses = this._addresses();
      
      if (!selectedId && addresses.length > 0) {
        const primaryAddress = addresses.find(addr => addr.isCommercial === false);
        const firstAddress = addresses[0];
        const selected = (primaryAddress?.addressId || firstAddress.addressId) || '';
        this._selectedAddressId.set(selected);
        this.addressSelected.emit(selected);
      }
    } catch (err) {
      this._error.set('Failed to load service addresses. Please try again.');
      console.error('Failed to load addresses:', err);
    } finally {
      this._loading.set(false);
    }
  }
  
  onAddressChange(event: Event) {
    const selectedId = (event.target as HTMLSelectElement).value;
    this._selectedAddressId.set(selectedId);
    this.addressSelected.emit(selectedId);
  }
}