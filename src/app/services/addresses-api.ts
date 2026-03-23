import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { ServiceAddress } from '../models/service-address.model';
import { tap } from 'rxjs/operators';
import { CacheInvalidator, CacheService } from './cache.service';

@Injectable({
  providedIn: 'root',
})
export class AddressesApi implements CacheInvalidator {

  constructor(private http: HttpClient, private cacheService: CacheService) {
    this.cacheService.registerCacheInvalidator(this);
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('accessToken');
    const headers: Record<string, string> = {};
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
    return new HttpHeaders(headers);
  }

  private cachedAddresses: ServiceAddress[] | null = null;

  invalidateCache(): void {
    this.cachedAddresses = null;
  }

  getAddresses(): Observable<ServiceAddress[]> {
    if (this.cachedAddresses) {
      return of([...this.cachedAddresses]);
    }

    return this.http.get<ServiceAddress[]>(`api/profile/address`, { headers: this.getAuthHeaders() }).pipe(
      tap((addresses: ServiceAddress[]) => {
        this.cachedAddresses = addresses;
      })
    );
  }

  createAddress(address: ServiceAddress): Observable<ServiceAddress> {
    this.invalidateCache();
    return this.http.post<ServiceAddress>(`api/profile/address`, address, { headers: this.getAuthHeaders() });
  }

  deleteAddress(addressId: string): Observable<void> {
    this.invalidateCache();
    return this.http.delete<void>(`api/profile/address/${addressId}`, { headers: this.getAuthHeaders() });
  }
}
