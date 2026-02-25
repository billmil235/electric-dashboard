import { Component, inject, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ServiceAddress } from '../../models/service-address.model';
import { ElectricCompany } from '../../models/electric-company.model';
import { LookupsApi } from '../../services/lookups-api';

@Component({
  selector: 'app-service-address-entry',
  imports: [CommonModule],
  templateUrl: './service-address-entry.html'
})
export class ServiceAddressEntry implements OnInit, OnChanges {

  @Input() address: ServiceAddress | null = null;

  @Output() addressSaved = new EventEmitter<ServiceAddress>();
  @Output() cancelled = new EventEmitter<void>();

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

  get isEditing(): boolean {
    return this._isEditing;
  }

  private _isEditing = false;
  private lookupsApi = inject(LookupsApi);
  electricCompanies = signal<ElectricCompany[]>([]);

  ngOnInit() {
    if (this.address) {
      this._isEditing = true;
      this.populateForm(this.address);
    }
    this.loadElectricCompanies();
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

  ngOnChanges(changes: SimpleChanges) {
    if (changes['address'] && this.address) {
      this.populateForm(this.address);
    }
  }

  onInputChange(field: any, value: string | boolean | number) {
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
      addressId: this._isEditing && this.address ? this.address.addressId : '',
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

    if (this._isEditing && addressToSave.addressId) {
      this.addressSaved.emit(addressToSave);
    } else {
      this.addressSaved.emit(addressToSave);
    }
  }

  onCancel() {
    this.cancelled.emit();
  }
}
