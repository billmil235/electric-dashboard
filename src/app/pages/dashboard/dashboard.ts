import { Component, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ServiceAddressSelector } from '../../components/service-address-selector/service-address-selector';
import { CommonModule } from '@angular/common';
import { Api } from '../../services/api';
import { ElectricBill } from '../../models/electric-bill.model';
import { ServiceAddress } from '../../models/service-address.model';
import { ConsumptionChartComponent } from '../../components/consumption-chart/consumption-chart.component';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, ServiceAddressSelector, CommonModule, ConsumptionChartComponent],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  username = signal('User');
  
  addresses = signal<ServiceAddress[]>([]);
  bills = signal<ElectricBill[]>([]);
  selectedAddressId = signal<string>('');
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
      },
      error: (err) => {
        console.error('Failed to load bills:', err);
      },
      complete: () => {
        this.loadingBills.set(false);
      }
    });
  }

  getChartData() {
    const bills = this.bills();
    if (bills.length === 0) {
      return [];
    }
    
    // Group bills by year
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
    return result.slice(0, 3);
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
}