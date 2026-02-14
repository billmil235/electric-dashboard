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
  billGuid = this.route.snapshot.params['billGuid'] || '';

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
  isEditing = false;

  pdfUploading = false;
  pdfUploadError: string | null = null;
  pdfUploaded = false;

  private selectedPdfFile: File | null = null;

  constructor() {
    // Check if we're editing an existing bill only if we have both parameters
    if (this.addressId && this.route.snapshot.params['billGuid']) {
      this.billGuid = this.route.snapshot.params['billGuid'] || '';
      this.isEditing = true;
      this.loadBillForEditing();
    } else {
      // If we're not editing, clear the form state (reset to empty form)
      this.clearForm();
    }
  }

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

      this.api.addElectricBill(this.addressId, request).subscribe({
        next: () => {
          this.success = true;
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          this.error = 'Failed to save billing information. Please try again.';
          this.loading = false;
        }
      });
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
      // Handle the Observable by subscribing to it
      this.api.uploadElectricBillPdf(this.addressId, this.selectedPdfFile).subscribe({
        next: (response) => {
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
        },
        error: (err) => {
          console.error('PDF upload failed:', err);
          this.pdfUploadError = 'Failed to process PDF. Please try again or enter information manually.';
          this.pdfUploading = false;
        }
      });
    } catch (err) {
      console.error('PDF upload failed:', err);
      this.pdfUploadError = 'Failed to process PDF. Please try again or enter information manually.';
      this.pdfUploading = false;
    } finally {
      // Note: will be set in the subscribe next handler, but we'll set it here for safety
      this.pdfUploading = false;
    }
  }

  clearForm() {
    this.billedDate = '';
    this.billedDateEnd = '';
    this.consumption = null;
    this.sentBack = null;
    this.billedAmount = null;
    this.unitPrice = null;
  }

  async loadBillForEditing() {
    if (!this.addressId || !this.billGuid) {
      this.error = 'Invalid bill information';
      return;
    }

    this.loading = true;
    try {
      // Handle the Observable by subscribing to it
      this.api.getBillByAddressAndGuid(this.addressId, this.billGuid).subscribe({
        next: (bill) => {
          console.log('Loaded bill for editing:', bill);

          if (bill?.length && bill[0]) {
            this.billedDate = bill[0].periodStartDate || '';
            this.billedDateEnd = bill[0].periodEndDate || '';
            this.consumption = bill[0].consumptionKwh ?? null;
            this.sentBack = bill[0].sentBackKwh ?? null;
            this.billedAmount = bill[0].billedAmount ?? null;
            this.unitPrice = bill[0].unitPrice ?? null;
          }
          this.loading = false;
          // Ensure change detection runs to update the UI
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.error = 'Failed to load bill information. Please try again.';
          console.error('Failed to load bill:', err);
          this.loading = false;
        }
      });
    } catch (err) {
      this.error = 'Failed to load bill information. Please try again.';
      console.error('Failed to load bill:', err);
      this.loading = false;
    } finally {
      // Set loading to false in the subscribe if already set
      // this.loading = false; // Already handled in subscribe
    }
  }
}