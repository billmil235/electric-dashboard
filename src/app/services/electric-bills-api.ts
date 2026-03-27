import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { ElectricBill } from '../models/electric-bill.model';
import { AuthHeadersService } from './auth-headers.service';
import { tap } from 'rxjs/operators';
import { CacheInvalidator, CacheService } from './cache.service';

@Injectable({
  providedIn: 'root',
})
export class ElectricBillsApi implements CacheInvalidator {
  private cachedBills: Record<string, ElectricBill[] | null> = {};

  constructor(
    private http: HttpClient,
    private authHeaders: AuthHeadersService,
    private cacheService: CacheService
  ) {
    this.cacheService.registerCacheInvalidator(this);
  }

  invalidateCache(): void {
    this.cachedBills = {};
  }

  addElectricBill(addressId: string, bill: ElectricBill): Observable<void> {
    this.invalidateCache();
    return this.http.post<void>(`api/data/electric-bill/${addressId}`, bill, { headers: this.authHeaders.getAuthHeaders() });
  }

  deleteElectricBill(addressId: string, billId: string): Observable<void> {
    this.invalidateCache();
    return this.http.delete<void>(`api/data/electric-bill/${addressId}/${billId}`, { headers: this.authHeaders.getAuthHeaders() });
  }


  getBillsByAddress(addressId: string): Observable<ElectricBill[]> {
    if (this.cachedBills[addressId] !== undefined && this.cachedBills[addressId] !== null) {
      return of([...(this.cachedBills[addressId] || [])]);
    }

    return this.http.get<ElectricBill[]>(`api/data/electric-bill/${addressId}`, { headers: this.authHeaders.getAuthHeaders() }).pipe(
      tap((bills: ElectricBill[]) => {
        this.cachedBills[addressId] = bills;
      })
    );
  }

  getBillByAddressAndGuid(addressId: string, billGuid: string): Observable<ElectricBill[]> {
    return this.http.get<ElectricBill[]>(`api/data/electric-bill/${addressId}/${billGuid}`, { headers: this.authHeaders.getAuthHeaders() });
  }

  updateElectricBill(addressId: string, billGuid: string, bill: ElectricBill): Observable<void> {
    this.invalidateCache();
    return this.http.put<void>(`api/data/electric-bill/${addressId}/${billGuid}`, bill, { headers: this.authHeaders.getAuthHeaders() });
  }

  uploadElectricBillPdf(addressId: string, file: File): Observable<ElectricBill> {
    return this.http.post<ElectricBill>(`api/data/electric-bill/upload/${addressId}`, file, { headers: this.authHeaders.getAuthHeaders(true) });
  }
}