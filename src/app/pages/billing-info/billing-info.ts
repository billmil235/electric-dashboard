import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Api } from '../../services/api';
import { Router, ActivatedRoute } from '@angular/router';
import { ServiceAddressSelector } from '../../components/service-address-selector/service-address-selector';

interface ElectricBill {
  addressId?: string | null;
  periodStartDate?: string;
  periodEndDate?: string;
  consumptionKwh?: number;
  sentBackKwh?: number | null;
  billedAmount?: number;
  unitPrice?: number | null;
}

@Component({
  selector: 'app-billing-info',
  imports: [CommonModule, FormsModule, ServiceAddressSelector],
  templateUrl: './billing-info.html',
  styleUrl: './billing-info.css',
})
export class BillingInfo {
  private readonly api = inject(Api);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  
  addressId = this.route.snapshot.params['addressGuid'] || '';
  
  billedDate = '';
  billedDateEnd = '';
  consumption: number | null = null;
  sentBack: number | null = null;
  billedAmount: number | null = null;
  unitPrice: number | null = null;
  
  loading = false;
  error: string | null = null;
  success = false;
  addressesLoading = false;
  
  constructor() {}

  async onSubmit() {
    this.loading = true;
    this.error = null;
    this.success = false;
    
    try {
      const request: ElectricBill = {
        addressId: this.addressId || null,
        periodStartDate: this.billedDate,
        periodEndDate: this.billedDateEnd,
        consumptionKwh: this.consumption || 0,
        sentBackKwh: this.sentBack || null,
        billedAmount: this.billedAmount || 0,
        unitPrice: this.unitPrice || null,
      };

      await this.api.addElectricBill(this.addressId, request).toPromise();
      this.success = true;
    } catch (err) {
      this.error = 'Failed to save billing information. Please try again.';
    } finally {
      this.loading = false;
    }
  }
  
  navigateToDashboard() {
    this.router.navigate(['/dashboard']);
  }
  
  onAddressSelected(addressId: string) {
    this.addressId = addressId;
  }
}