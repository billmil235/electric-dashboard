import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Api } from '../../services/api';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { ServiceAddressSelector } from '../../components/service-address-selector/service-address-selector';
import { ElectricBill } from '../../models/electric-bill.model';
import { LoggedInLayout } from "../logged-in-layout/logged-in-layout";

@Component({
  selector: 'app-billing-info',
  imports: [CommonModule, FormsModule, ServiceAddressSelector, LoggedInLayout],
  templateUrl: './billing-info.html',
  styleUrl: './billing-info.css',
})
export class BillingInfo {
  private readonly api = inject(Api);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly cdr = inject(ChangeDetectorRef);

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

  pdfUploading = false;
  pdfUploadError: string | null = null;
  pdfUploaded = false;

  private selectedPdfFile: File | null = null;

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
      this.router.navigate(['/dashboard']);
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

  onPdfSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedPdfFile = input.files[0];
      this.pdfUploaded = false;
      this.pdfUploadError = null;
      this.uploadPdf();
    }
  }

  private async uploadPdf() {
    if (!this.selectedPdfFile || !this.addressId) {
      this.pdfUploadError = 'Please select a PDF file and an address';
      return;
    }

    this.pdfUploading = true;
    this.pdfUploadError = null;

    try {
      const response = await this.api.uploadElectricBillPdf(this.addressId, this.selectedPdfFile).toPromise();

      if (response) {
        this.billedDate = response.periodStartDate || '';
        this.billedDateEnd = response.periodEndDate || '';
        this.consumption = response.consumptionKwh ?? null;
        this.sentBack = response.sentBackKwh ?? null;
        this.billedAmount = response.billedAmount ?? null;
        this.unitPrice = response.unitPrice ?? null;
        
        // Trigger change detection manually to update the form
        this.cdr.detectChanges();
        
        this.pdfUploaded = true;
      }
    } catch (err) {
      console.error('PDF upload failed:', err);
      this.pdfUploadError = 'Failed to process PDF. Please try again or enter information manually.';
    } finally {
      this.pdfUploading = false;
    }
  }
}