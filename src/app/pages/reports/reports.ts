import { Component, OnInit, signal } from '@angular/core';
import { LoggedInLayout } from "../logged-in-layout/logged-in-layout";
import { MonthlyConsumptionGraphComponent } from '../../components/monthly-consumption-graph/monthly-consumption-graph.component';
import { Api } from '../../services/api';
import { ServiceAddress } from '../../models/service-address.model';
import { ElectricBill } from '../../models/electric-bill.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ServiceAddressSelector } from '../../components/service-address-selector/service-address-selector';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.html',
  styleUrl: './reports.css',
  standalone: true,
  imports: [LoggedInLayout, MonthlyConsumptionGraphComponent, ServiceAddressSelector, CommonModule, FormsModule]
})
export class Reports implements OnInit {
  loading = false;
  monthlyData: any[] = [];
  selectedAddressId: string = '';
  addresses = signal<ServiceAddress[]>([]);
  
  constructor(private api: Api) { }

  ngOnInit() {
    // Address selection will be handled by the component
  }
  
  onAddressesLoaded(addresses: ServiceAddress[]) {
    if (addresses.length > 0) {
      const primaryAddress = addresses.find(addr => addr.isCommercial === false);
      const firstAddress = addresses[0];
      this.selectedAddressId = primaryAddress?.addressId || firstAddress.addressId;
      this.loadBillsForAddress(this.selectedAddressId);
    }
  }
  
  onAddressSelected(addressId: string) {
    this.selectedAddressId = addressId;
    if (this.selectedAddressId) {
      this.loadBillsForAddress(this.selectedAddressId);
    } else {
      this.monthlyData = [];
    }
  }
  
  loadBillsForAddress(addressId: string) {
    this.loading = true;
    this.api.getBillsByAddress(addressId).subscribe({
      next: (bills) => {
        console.log('Bills loaded:', bills); // Debug log
        this.processBillsForMonthlyData(bills);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading bills:', error);
        this.loading = false;
      }
    });
  }
  
  processBillsForMonthlyData(bills: ElectricBill[]) {
    console.log('Processing bills for monthly data:', bills); // Debug log
    // Process bills to create monthly consumption data for the last 3 years
    const data = [];
    const currentYear = new Date().getFullYear();
    const threeYearsAgo = currentYear - 2;
    
    // Group bills by year/month to calculate total consumption per month
    const monthlyTotals: { [key: string]: number } = {};
    
    bills.forEach(bill => {
      if (!bill.serviceYear || !bill.consumptionKwh) return;
      
      // Only process bills from the last 3 years
      if (bill.serviceYear >= threeYearsAgo && bill.serviceYear <= currentYear) {
        const month = bill.serviceMonth || 1; // Default to January if month is missing
        
        // Create a key for year/month combination
        const key = `${bill.serviceYear}-${month}`;
        
        if (!monthlyTotals[key]) {
          monthlyTotals[key] = 0;
        }
        
        // Add consumption to monthly total
        monthlyTotals[key] += bill.consumptionKwh;
      }
    });
    
    // Generate all months for the last 3 years with actual or zero consumption
    for (let year = threeYearsAgo; year <= currentYear; year++) {
      for (let month = 1; month <= 12; month++) {
        const key = `${year}-${month}`;
        const consumption = monthlyTotals[key] || 0;
        
        data.push({
          year: year,
          month: month,
          consumption: consumption
        });
      }
    }
    
    this.monthlyData = data;
    console.log('Processed monthly data:', data); // Debug log
  }
}
