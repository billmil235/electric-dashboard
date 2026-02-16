import { Component, OnInit, signal } from '@angular/core';
import { LoggedInLayout } from "../logged-in-layout/logged-in-layout";
import { MonthlyConsumptionGraphComponent } from '../../components/monthly-consumption-graph/monthly-consumption-graph.component';
import { Api } from '../../services/api';
import { ServiceAddress } from '../../models/service-address.model';
import { ElectricBill } from '../../models/electric-bill.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ServiceAddressSelector } from '../../components/service-address-selector/service-address-selector';
import { sign } from 'chart.js/helpers';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.html',
  styleUrl: './reports.css',
  standalone: true,
  imports: [LoggedInLayout, MonthlyConsumptionGraphComponent, ServiceAddressSelector, CommonModule, FormsModule]
})
export class Reports implements OnInit {
  loading = false;
  selectedAddressId: string = '';
  addresses = signal<ServiceAddress[]>([]);
  bills = signal<ElectricBill[]>([]);

  constructor(private api: Api) { }

  ngOnInit() {
    // Address selection will be handled by the component
  }
  
  onAddressesLoaded(addresses: ServiceAddress[]) {
    if (addresses.length > 0) {
      this.selectedAddressId = addresses[0].addressId;
      this.loadBillsForAddress();
    }
  }
  
  onAddressSelected(addressId: string) {
    this.selectedAddressId = addressId;
    if (this.selectedAddressId) {
      this.loadBillsForAddress();
    }
  }
  
  loadBillsForAddress() {
    this.loading = true;
    console.log('Loading bills for address ID:', this.selectedAddressId); // Debug log
    this.api.getBillsByAddress(this.selectedAddressId).subscribe({
      next: (bills) => {
        console.log('Bills loaded:', bills); // Debug log
        this.bills.set(bills);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading bills:', error);
        this.loading = false;
      }
    });
  }
}
