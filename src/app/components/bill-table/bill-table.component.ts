import { Component, output, input, ChangeDetectionStrategy } from '@angular/core';
import { ElectricBill } from '../../models/electric-bill.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-bill-table',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  templateUrl: './bill-table.component.html',
  styleUrls: ['./bill-table.component.css']
})
export class BillTableComponent {
  bills = input<ElectricBill[]>([]);
  loading = input(false);
  editBill = output<ElectricBill>();

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