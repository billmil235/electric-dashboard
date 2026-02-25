import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ElectricBill } from '../../models/electric-bill.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-bill-table',
  imports: [CommonModule],
  template: `
    <div class="bills-section">
      @if (loading) {
        <div class="loading">Loading bills...</div>
      } @else if (bills.length === 0) {
        <div class="no-bills">No bills found for this address</div>
      } @else {
        <div class="table-responsive">
          <table class="bills-table">
            <thead>
              <tr>
                <th>Year</th>
                <th>Period</th>
                <th>Consumption (kWh)</th>
                <th>Sent Back (kWh)</th>
                <th>Amount</th>
                <th>Unit Price</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (bill of bills; track bill.addressId || bill.periodStartDate) {
                <tr>
                  <td data-label="Year">{{ bill.serviceYear }}</td>
                  <td data-label="Period">
                    {{ formatDate(bill.periodStartDate) }} - {{ formatDate(bill.periodEndDate) }}
                  </td>
                  <td data-label="Consumption (kWh)">{{ formatNumber(bill.consumptionKwh) }}</td>
                  <td data-label="Sent Back (kwh)">{{ formatNumber(bill.sentBackKwh) }}</td>
                  <td data-label="Amount">
                    {{ formatCurrency(bill.billedAmount) }}
                  </td>
                  <td data-label="Unit Price">
                    {{ formatCurrency(bill.unitPrice) }}
                  </td>
                  <td data-label="Actions">
                    <button class="action-btn edit" (click)="onEditBill(bill)">Edit</button>
                    <button class="action-btn delete">Delete</button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
  styleUrls: ['./bill-table.component.css']
})
export class BillTableComponent {
  @Input() bills: ElectricBill[] = [];
  @Input() loading = false;
  @Output() editBill = new EventEmitter<ElectricBill>();

  formatDate(dateString?: string): string {
    if (!dateString) return '';
    
    // Parse the date and format it in UTC without converting to local time
    const date = new Date(dateString);
    
    // Format to ISO string and extract just the date part (YYYY-MM-DD)
    // This ensures no timezone conversion happens
    return date.toISOString().split('T')[0];
  }

  formatCurrency(amount?: number | null): string {
    if (amount === undefined || amount === null) return '';
    return Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  formatNumber(value?: number | null): string {
    if (value === undefined || value === null) return '';
    return new Intl.NumberFormat('en-US').format(value);
  }

  onEditBill(bill: ElectricBill) {
    this.editBill.emit(bill);
  }
}