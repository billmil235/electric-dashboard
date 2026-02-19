import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ElectricBill } from '../models/electric-bill.model';

@Injectable({
  providedIn: 'root',
})
export class ElectricBillsApi {

  constructor(private http: HttpClient) {}

  private getAuthHeaders(isFormData?: boolean): HttpHeaders {
    const token = localStorage.getItem('accessToken');
    const headers: { [key: string]: string } = {};
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
    if (!isFormData) {
      headers['Content-Type'] = 'application/json';
    }
    return new HttpHeaders(headers);
  }

  addElectricBill(addressId: string, bill: ElectricBill): Observable<void> {
    return this.http.post<void>(`api/data/electric-bill/${addressId}`, bill, { headers: this.getAuthHeaders() });
  }

  getBillsByAddress(addressId: string): Observable<ElectricBill[]> {
    return this.http.get<ElectricBill[]>(`api/data/electric-bill/${addressId}`, { headers: this.getAuthHeaders() });
  }

  getBillByAddressAndGuid(addressId: string, billGuid: string): Observable<ElectricBill[]> {
    return this.http.get<ElectricBill[]>(`api/data/electric-bill/${addressId}/${billGuid}`, { headers: this.getAuthHeaders() });
  }

  updateElectricBill(addressId: string, billGuid: string, bill: ElectricBill): Observable<void> {
    return this.http.put<void>(`api/data/electric-bill/${addressId}/${billGuid}`, bill, { headers: this.getAuthHeaders() });
  }

  uploadElectricBillPdf(addressId: string, file: File): Observable<ElectricBill> {
    const formData = new FormData();
    formData.append('file', file);
    
    return this.http.post<ElectricBill>(`api/data/electric-bill/upload/${addressId}`, formData, { headers: this.getAuthHeaders(true) });
  }
}
