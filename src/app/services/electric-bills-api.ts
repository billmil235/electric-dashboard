import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ElectricBill } from '../models/electric-bill.model';
import { AuthHeadersService } from './auth-headers.service';

@Injectable({
  providedIn: 'root',
})
export class ElectricBillsApi {

  constructor(
    private http: HttpClient,
    private authHeaders: AuthHeadersService
  ) {}

  addElectricBill(addressId: string, bill: ElectricBill): Observable<void> {
    return this.http.post<void>(`api/data/electric-bill/${addressId}`, bill, { headers: this.authHeaders.getAuthHeaders() });
  }

  getBillsByAddress(addressId: string): Observable<ElectricBill[]> {
    return this.http.get<ElectricBill[]>(`api/data/electric-bill/${addressId}`, { headers: this.authHeaders.getAuthHeaders() });
  }

  getBillByAddressAndGuid(addressId: string, billGuid: string): Observable<ElectricBill[]> {
    return this.http.get<ElectricBill[]>(`api/data/electric-bill/${addressId}/${billGuid}`, { headers: this.authHeaders.getAuthHeaders() });
  }

  updateElectricBill(addressId: string, billGuid: string, bill: ElectricBill): Observable<void> {
    return this.http.put<void>(`api/data/electric-bill/${addressId}/${billGuid}`, bill, { headers: this.authHeaders.getAuthHeaders() });
  }

  uploadElectricBillPdf(addressId: string, file: File): Observable<ElectricBill> {
    const formData = new FormData();
    formData.append('file', file);
    
    return this.http.post<ElectricBill>(`api/data/electric-bill/upload/${addressId}`, formData, { headers: this.authHeaders.getAuthHeaders(true) });
  }
}