import { Component, inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Api } from '../../services/api';
import { ServiceAddress } from '../../models/service-address.model';

@Component({
  selector: 'app-service-address-selector',
  imports: [CommonModule],
  templateUrl: './service-address-selector.html',
  styleUrls: ['./service-address-selector.css']
})

export class ServiceAddressSelector implements OnInit {
  private readonly api = inject(Api);
  
  @Input() selectedAddressId: string = '';
  @Input() loading: boolean = false;
  @Output() addressSelected = new EventEmitter<string>();
  @Output() addressesLoaded = new EventEmitter<ServiceAddress[]>();
  
  addresses: ServiceAddress[] = [];
  error: string | null = null;
  
  async ngOnInit() {
    await this.loadAddresses();
  }
  
  async loadAddresses() {
    this.loading = true;
    this.error = null;
    
    try {
      // Fetch addresses from the API endpoint
      const response = await this.api.getAddresses().toPromise();
      this.addresses = response || [];
      this.addressesLoaded.emit(this.addresses);
      
      // If no selected address is set, select the first one or the primary one
      if (!this.selectedAddressId && this.addresses.length > 0) {
        const primaryAddress = this.addresses.find(addr => addr.isCommercial === false);
        const firstAddress = this.addresses[0];
        this.selectedAddressId = primaryAddress?.addressId || firstAddress.addressId;
        this.addressSelected.emit(this.selectedAddressId);
      }
    } catch (err) {
      this.error = 'Failed to load service addresses. Please try again.';
      console.error('Failed to load addresses:', err);
    } finally {
      this.loading = false;
    }
  }
  
  onAddressChange(event: Event) {
    const selectedId = (event.target as HTMLSelectElement).value;
    this.selectedAddressId = selectedId;
    this.addressSelected.emit(selectedId);
  }
}