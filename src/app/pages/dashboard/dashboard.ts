import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ServiceAddressSelector } from '../../components/service-address-selector/service-address-selector';
import { CommonModule } from '@angular/common';
import { ElectricBillsApi } from '../../services/electric-bills-api';
import { ElectricBill } from '../../models/electric-bill.model';
import { ServiceAddress } from '../../models/service-address.model';
import { ConsumptionChartComponent } from '../../components/consumption-chart/consumption-chart.component';
import { BillTableComponent } from '../../components/bill-table/bill-table.component';
import { LoggedInLayout } from '../logged-in-layout/logged-in-layout';
import { ForecastApiService } from '../../services/forecast-api.service';
import { Forecast } from '../../models/forecast.model';
import { ForecastDisplay } from '../../components/forecast-display/forecast-display';

@Component({
  selector: 'app-dashboard',
  imports: [ServiceAddressSelector, CommonModule, ConsumptionChartComponent, BillTableComponent, LoggedInLayout, ForecastDisplay],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  username = signal('User');
  addresses = signal<ServiceAddress[]>([]);
  bills = signal<ElectricBill[]>([]);
  selectedAddressId = signal<string>('');
  selectedYearFilter = signal<string>('all');
  selectedChartView = signal<'yearly' | 'ytd'>('yearly');
  loadingBills = signal<boolean>(false);

  forecast = signal<Forecast | null>(null);

  constructor(
    private router: Router,
    private electricBillsApi: ElectricBillsApi,
    private forecastApi: ForecastApiService,
  ) {}

  logout() {
    localStorage.removeItem('accessToken');
    this.router.navigate(['/']);
  }

  onAddressesLoaded(addresses: ServiceAddress[]) {
    this.addresses.set(addresses);
    if (addresses.length) {
      const first = addresses[0];
      this.selectedAddressId.set(first.addressId);
      this.loadBills(first.addressId);
      this.fetchForecast(first.addressId);
    }
  }

  onAddressSelected(id: string) {
    this.selectedAddressId.set(id);
    this.loadBills(id);
    this.fetchForecast(id);
  }

  loadBills(id: string) {
    this.loadingBills.set(true);
    this.electricBillsApi.getBillsByAddress(id).subscribe({
      next: (bills) => {
        this.bills.set(bills);
        this.updateYearFilterOptions();
      },
      error: err => console.error('Failed to load bills:', err),
      complete: () => this.loadingBills.set(false),
    });
  }

  fetchForecast(id: string) {
    this.forecastApi.getForecast(id).subscribe({
      next: (f: Forecast) => this.forecast.set(f),
      error: (err: any) => { console.error('Forecast error', err); this.forecast.set(null); },
    });
  }

  updateYearFilterOptions() {
    const bills = this.bills();
    const years = Array.from(
      new Set(bills.map(b => b.serviceYear).filter(y => y != null)),
    ).sort((a, b) => b - a);
    if (this.selectedYearFilter() !== 'all' && !years.includes(Number(this.selectedYearFilter()))) {
      this.selectedYearFilter.set('all');
    }
  }

  get filteredBills() {
    const bills = this.bills();
    const filter = this.selectedYearFilter();
    if (filter === 'all') return bills;
    return bills.filter(b => b.serviceYear === Number(filter));
  }

  get yearOptions() {
    const bills = this.bills();
    const years = Array.from(
      new Set(bills.map(b => b.serviceYear).filter(y => y != null)),
    ).sort((a, b) => b - a);
    return [{ value: 'all', label: 'All Years' }, ...years.map(y => ({ value: y.toString(), label: y.toString() }))];
  }

  onYearSelected(e: Event) {
    const v = (e.target as HTMLSelectElement).value;
    this.selectedYearFilter.set(v);
  }

  onEditBill(bill: ElectricBill) {
    if (this.selectedAddressId() && bill && bill.billId) {
      this.router.navigate(['/dashboard/billing', this.selectedAddressId(), bill.billId]);
    }
  }

  setChartView(view: 'yearly' | 'ytd') { this.selectedChartView.set(view); }

  getChartData() {
    const bills = this.filteredBills;
    if (!bills.length) return [];
    if (this.selectedChartView() === 'ytd') return this.getYTDData();
    return this.groupBillsByYear(bills).sort((a,b)=> Number(b.label) - Number(a.label));
  }

  private getYTDData(): Array<{ label: string; totalConsumption: number; totalSentBack: number; totalBilledAmount: number; }> {
    const bills = this.bills();
    const now = new Date();
    const currentYear = now.getFullYear();
    const months = new Set<number>();
    for (const b of bills) {
      if (b.serviceYear === currentYear && b.serviceMonth) months.add(b.serviceMonth);
    }
    if (!months.size) return [];
    const map = new Map<number, { totalConsumption: number; totalSentBack: number; totalBilledAmount: number; }>();
    for (const b of bills) {
      if (!months.has(b.serviceMonth!)) continue;
      const year = b.serviceYear!;
      const entry = map.get(year) || { totalConsumption: 0, totalSentBack: 0, totalBilledAmount: 0 };
      entry.totalConsumption += b.consumptionKwh ?? 0;
      entry.totalSentBack += b.sentBackKwh ?? 0;
      entry.totalBilledAmount += b.billedAmount ?? 0;
      map.set(year, entry);
    }
    const res = Array.from(map.entries()).map(([y, v]) => ({ label: y.toString(), totalConsumption: v.totalConsumption, totalSentBack: v.totalSentBack, totalBilledAmount: v.totalBilledAmount }));
    res.sort((a, b) => Number(b.label) - Number(a.label));
    return res;
  }

  private groupBillsByYear(bills: ElectricBill[]): Array<{ label: string; totalConsumption: number; totalSentBack: number; totalBilledAmount: number; }> {
    const map = new Map<number, { totalConsumption: number; totalSentBack: number; totalBilledAmount: number; }>();
    for (const b of bills) {
      const year = b.serviceYear!;
      const entry = map.get(year) || { totalConsumption: 0, totalSentBack: 0, totalBilledAmount: 0 };
      entry.totalConsumption += b.consumptionKwh ?? 0;
      entry.totalSentBack += b.sentBackKwh ?? 0;
      entry.totalBilledAmount += b.billedAmount ?? 0;
      map.set(year, entry);
    }
    return Array.from(map.entries()).map(([y, v]) => ({ label: y.toString(), totalConsumption: v.totalConsumption, totalSentBack: v.totalSentBack, totalBilledAmount: v.totalBilledAmount }));
  }

  trackByYear(index: number, option: any) { return option.value; }
}
