import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthHeadersService } from './auth-headers.service';
import { Observable } from 'rxjs';
import { Forecast } from '../models/forecast.model';

@Injectable({ providedIn: 'root' })
export class ForecastApiService {
  constructor(
    private http: HttpClient,
    private authHeaders: AuthHeadersService
  ) {}

  getForecast(addressId: string): Observable<Forecast> {
    const headers = this.authHeaders.getAuthHeaders();
    return this.http.get<Forecast>(`/api/forecast/${addressId}`, { headers });
  }
}
