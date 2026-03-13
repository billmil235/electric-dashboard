import { Component, inject, input, output, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ServiceAddress } from '../../models/service-address.model';
import { ElectricCompany } from '../../models/electric-company.model';
import { LookupsApi } from '../../services/lookups-api';

@Component({
  selector: 'app-service-address-entry',
  imports: [CommonModule],
  templateUrl: './service-address-entry.html'
})
export class ServiceAddressEntry {
  address = input<ServiceAddress | null>(null);
  addressSaved = output<ServiceAddress>();
  cancelled = output<void>();

  form = signal({
    addressName: '',
    addressLine1: '',
    addressLine2: '',
    city: '',
    state: '',
    zipCode: '',
    country: '',
    isCommercial: false,
    electricCompanyId: 0
  });

  get isEditing() {
    return this._isEditing();
  }

  private _isEditing = signal(false);
  private lookupsApi = inject(LookupsApi);
  electricCompanies = signal<ElectricCompany[]>([]);

  constructor() {
    // Handle address changes
    effect(() => {
      this._isEditing.set(this.address() !== null);
      this.loadElectricCompanies();
    });

    // Watch for address input to populate form
    effect(() => {
      if (this.address()) {
        this.populateForm(this.address()!);
      }
    });
  }

  loadElectricCompanies() {
    this.lookupsApi.getElectricCompanies().subscribe(companies => {
      this.electricCompanies.set(companies);
    });
  }

  populateForm(address: ServiceAddress) {
    this.form.set({
      addressName: address.addressName,
      addressLine1: address.addressLine1,
      addressLine2: address.addressLine2 || '',
      city: address.city,
      state: address.state,
      zipCode: address.zipCode,
      country: address.country || '',
      isCommercial: address.isCommercial,
      electricCompanyId: address.electricCompanyId
    });
  }

  onInputChange(field: string, value: string | boolean | number) {
    this.form.update(current => ({
      ...current,
      [field]: value
    }));
  }

  onSelectChange(event: Event, field: string) {
    const value = (event.target as HTMLSelectElement).value;
    this.onInputChange(field, value === '' ? 0 : parseInt(value, 10));
  }

  onSave() {
    const formData = this.form();
    
    const addressToSave: ServiceAddress = {
      addressId: this._isEditing() && this.address() ? this.address()!.addressId : '',
      addressName: formData.addressName,
      addressLine1: formData.addressLine1,
      addressLine2: formData.addressLine2 || null,
      city: formData.city,
      state: formData.state,
      zipCode: formData.zipCode,
      country: formData.country || null,
      isCommercial: formData.isCommercial,
      electricCompanyId: formData.electricCompanyId
    };

    if (this._isEditing() && addressToSave.addressId) {
      this.addressSaved.emit(addressToSave);
    } else {
      this.addressSaved.emit(addressToSave);
    }
  }

  onCancel() {
    this.cancelled.emit();
  }
}
