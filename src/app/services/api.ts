import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

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

interface LoginRequest {
  username: string;
  password: string;
}

interface UserRegistration {
  emailAddress: string;
  password: string;
  dateOfBirth: Date;
  firstName: string;
  lastName: string;
}

interface ElectricBill {
  addressId?: string | null;
  periodStartDate?: string;
  periodEndDate?: string;
  consumptionKwh?: number;
  sentBackKwh?: number | null;
  billedAmount?: number;
  unitPrice?: number | null;
}

interface Address {
  addressId: string;
  addressName: string;
  addressLine1: string;
  addressLine2: string | null;
  city: string;
  state: string;
  zipCode: string;
  country: string | null;
  isCommercial: boolean;
}

interface ServiceAddress {
  id: string;
  streetAddress: string;
  city: string;
  state: string;
  zipCode: string;
  isPrimary: boolean;
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

  getAddresses(): Observable<Address[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<Address[]>(`api/profile/address`, { headers });
  }
}
