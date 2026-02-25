import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ElectricCompany } from '../models/electric-company.model';
import { AuthHeadersService } from './auth-headers.service';

@Injectable({
  providedIn: 'root',
})
export class LookupsApi {
  constructor(
    private http: HttpClient,
    private authHeaders: AuthHeadersService
  ) {}

  getElectricCompanies(): Observable<ElectricCompany[]> {
    return this.http.get<ElectricCompany[]>(`api/lookups/electric-companies`, { headers: this.authHeaders.getAuthHeaders() })
  }
}