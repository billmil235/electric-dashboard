import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthHeadersService } from './auth-headers.service';
import { Observable } from 'rxjs';

export interface CsvHeaderDto {
  headers: string[];
}

export interface SolarDataDto {
  date: string;
  value: number;
}

@Injectable({ providedIn: 'root' })
export class SolarGenerationService {
  constructor(
    private http: HttpClient,
    private authHeaders: AuthHeadersService
  ) {}

  uploadHeader(file: File): Observable<CsvHeaderDto> {
    const form = new FormData();
    form.append('file', file);
    const headers = this.authHeaders.getAuthHeaders(true);
    return this.http.post<CsvHeaderDto>('/api/solar-generation/header', form, { headers });
  }

  uploadData(file: File, dateCol: string, valueCol: string): Observable<SolarDataDto[]> {
    const form = new FormData();
    form.append('file', file);
    form.append('dateColumn', dateCol);
    form.append('valueColumn', valueCol);
    const headers = this.authHeaders.getAuthHeaders(true);
    return this.http.post<SolarDataDto[]>('/api/solar-generation/data', form, { headers });
  }
}
