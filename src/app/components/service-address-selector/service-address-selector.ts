import { Component, inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddressesApi } from '../../services/addresses-api';
import { ServiceAddress } from '../../models/service-address.model';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-service-address-selector',
  imports: [CommonModule],
  templateUrl: './service-address-selector.html',
  styleUrls: ['./service-address-selector.css']
})

export class ServiceAddressSelector implements OnInit {
  private readonly addressesApi = inject(AddressesApi);
  private _loading: boolean = false;
  
  get loading(): boolean {
    return this._loading;
  }
  
  @Input() set selectedAddressId(value: string) {
    this._selectedAddressId = value;
  }
  get selectedAddressId(): string {
    return this._selectedAddressId;
  }
  
  @Input() set loading(value: boolean) {
    this._loading = value;
  }
  
  @Output() addressSelected = new EventEmitter<string>();
  @Output() addressesLoaded = new EventEmitter<ServiceAddress[]>();
  
  addresses: ServiceAddress[] = [];
  error: string | null = null;
  private _selectedAddressId: string = '';
  
  async ngOnInit() {
    await this.loadAddresses();
  }
  
  async loadAddresses() {
    this._loading = true;
    this.error = null;
    
    try {
      const response = await this.addressesApi.getAddresses().pipe(take(1)).toPromise();
      this.addresses = response || [];
      this.addressesLoaded.emit(this.addresses);
      
      if (!this._selectedAddressId && this.addresses.length > 0) {
        const primaryAddress = this.addresses.find(addr => addr.isCommercial === false);
        const firstAddress = this.addresses[0];
        this._selectedAddressId = (primaryAddress?.addressId || firstAddress.addressId)!;
        this.addressSelected.emit(this._selectedAddressId);
      }
    } catch (err) {
      this.error = 'Failed to load service addresses. Please try again.';
      console.error('Failed to load addresses:', err);
    } finally {
      this._loading = false;
    }
  }
  
  onAddressChange(event: Event) {
    const selectedId = (event.target as HTMLSelectElement).value;
    this.selectedAddressId = selectedId;
    this.addressSelected.emit(selectedId);
  }
}