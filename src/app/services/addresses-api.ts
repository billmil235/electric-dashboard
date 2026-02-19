import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceAddress } from '../models/service-address.model';

@Injectable({
  providedIn: 'root',
})
export class AddressesApi {

  constructor(private http: HttpClient) {}

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('accessToken');
    const headers: { [key: string]: string } = {};
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
    return new HttpHeaders(headers);
  }

  getAddresses(): Observable<ServiceAddress[]> {
    return this.http.get<ServiceAddress[]>(`api/profile/address`, { headers: this.getAuthHeaders() });
  }

  createAddress(address: ServiceAddress): Observable<ServiceAddress> {
    return this.http.post<ServiceAddress>(`api/profile/address`, address, { headers: this.getAuthHeaders() });
  }

  updateAddress(addressId: string, address: ServiceAddress): Observable<ServiceAddress> {
    return this.http.put<ServiceAddress>(`api/profile/address/${addressId}`, address, { headers: this.getAuthHeaders() });
  }
}
