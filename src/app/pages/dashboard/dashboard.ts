import { Component, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ServiceAddressSelector } from '../../components/service-address-selector/service-address-selector';
import { CommonModule } from '@angular/common';
import { Api } from '../../services/api';
import { ElectricBill } from '../../models/electric-bill.model';
import { ServiceAddress } from '../../models/service-address.model';
import { ConsumptionChartComponent } from '../../components/consumption-chart/consumption-chart.component';
import { BillTableComponent } from '../../components/bill-table/bill-table.component';
import { LoggedInLayout } from '../logged-in-layout/logged-in-layout';

@Component({
  selector: 'app-dashboard',
  imports: [ServiceAddressSelector, CommonModule, ConsumptionChartComponent, BillTableComponent, LoggedInLayout],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  username = signal('User');
  
  addresses = signal<ServiceAddress[]>([]);
  bills = signal<ElectricBill[]>([]);
  selectedAddressId = signal<string>('');
  selectedYearFilter = signal<string>('all');
  loadingBills = signal<boolean>(false);
  
  constructor(private router: Router, private api: Api) {}

  logout() {
    localStorage.removeItem('accessToken');
    this.router.navigate(['/']);
  }

  onAddressesLoaded(addresses: ServiceAddress[]) {
    this.addresses.set(addresses);
    if (addresses.length > 0) {
      const primaryAddress = addresses.find(addr => addr.isCommercial === false);
      const firstAddress = addresses[0];
      this.selectedAddressId.set(primaryAddress?.addressId || firstAddress.addressId);
      this.loadBills(this.selectedAddressId());
    }
  }

  onAddressSelected(addressId: string) {
    this.selectedAddressId.set(addressId);
    this.loadBills(addressId);
  }

  loadBills(addressId: string) {
    this.loadingBills.set(true);
    this.api.getBillsByAddress(addressId).subscribe({
      next: (bills) => {
        this.bills.set(bills);
        this.updateYearFilterOptions();
      },
      error: (err) => {
        console.error('Failed to load bills:', err);
      },
      complete: () => {
        this.loadingBills.set(false);
      }
    });
  }

  updateYearFilterOptions() {
    const bills = this.bills();
    const uniqueYears = Array.from(new Set(bills.map(bill => bill.serviceYear).filter(year => year !== null && year !== undefined)))
      .sort((a, b) => b - a);
    
    const selectedYear = this.selectedYearFilter();
    if (selectedYear !== 'all' && !uniqueYears.includes(Number(selectedYear))) {
      this.selectedYearFilter.set('all');
    }
  }

  get filteredBills() {
    const bills = this.bills();
    const yearFilter = this.selectedYearFilter();
    
    if (yearFilter === 'all') {
      return bills;
    }
    
    return bills.filter(bill => bill.serviceYear === Number(yearFilter));
  }

  get yearOptions() {
    const bills = this.bills();
    const uniqueYears = Array.from(new Set(bills.map(bill => bill.serviceYear).filter(year => year !== null && year !== undefined)))
      .sort((a, b) => b - a);
    
    return [{ value: 'all', label: 'All Years' }, ...uniqueYears.map(year => ({ value: year.toString(), label: year.toString() }))];
  }

  onYearSelected(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.selectedYearFilter.set(value);
  }

  onEditBill(bill: ElectricBill) {
    // Navigate to the edit route including billGuid
    if (this.selectedAddressId() && bill && bill.billId) {
      this.router.navigate(['/dashboard/billing', this.selectedAddressId(), bill.billId]);
    }
  }

  getChartData() {
    const bills = this.filteredBills;
    if (bills.length === 0) {
      return [];
    }
    
    const yearlyData = this.groupBillsByYear(bills);
    
    if (yearlyData.length === 0) {
      return [];
    }
    
    return yearlyData;
  }

  private groupBillsByYear(bills: ElectricBill[]): Array<{
    year: number;
    totalConsumption: number;
    totalSentBack: number;
    totalBilledAmount: number;
  }> {
    const yearlyMap = new Map<number, {
      year: number;
      totalConsumption: number;
      totalSentBack: number;
      totalBilledAmount: number;
    }>();
    
    for (const bill of bills) {
      // Use serviceYear directly since it's always present
      const year = bill.serviceYear;
      
      if (!year) continue;
      
      let yearlyData = yearlyMap.get(year);
      if (!yearlyData) {
        yearlyData = {
          year,
          totalConsumption: 0,
          totalSentBack: 0,
          totalBilledAmount: 0
        };
        yearlyMap.set(year, yearlyData);
      }
      
      yearlyData.totalConsumption += (bill.consumptionKwh || 0);
      yearlyData.totalSentBack += (bill.sentBackKwh || 0);
      yearlyData.totalBilledAmount += (bill.billedAmount || 0);
    }
    
    // Convert map to array and sort by year descending (most recent first)
    const result = Array.from(yearlyMap.values());
    result.sort((a, b) => b.year - a.year);
    
    // Get only the last 3 years
    return result.slice(0, 4);
  }

  formatDate(dateString?: string): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString();
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

  trackByYear(index: number, option: { value: string; label: string }): string {
    return option.value;
  }
}
