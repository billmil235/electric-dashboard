import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ElectricBill } from '../models/electric-bill.model';
import { ServiceAddress } from '../models/service-address.model';
import { UserRegistration } from '../models/user-registration.model';
import { LoginRequest } from '../models/login-request.model';

interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  tokenType?: string;
  expiresIn?: number;
  refreshExpiresIn?: number;
  notBeforePolicy?: number;
  sessionState?: string;
  scope?: string;
}

@Injectable({
  providedIn: 'root',
})
export class Api {
  private baseUrl = 'api/users';

  constructor(private http: HttpClient) {}

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('accessToken');
    console.log('Retrieved token from localStorage:', token); // Debug log
    if (token) {
      return new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      });
    }
    return new HttpHeaders({
      'Content-Type': 'application/json'
    });
  }

  register(user: UserRegistration): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/register`, user);
  }

  addElectricBill(addressId: string, bill: ElectricBill): Observable<void> {
    const headers = this.getAuthHeaders();
    return this.http.post<void>(`${this.baseUrl}/${addressId}/bill`, bill, { headers });
  }

  getAddresses(): Observable<ServiceAddress[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<ServiceAddress[]>(`api/profile/address`, { headers });
  }
}
