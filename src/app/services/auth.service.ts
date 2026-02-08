import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
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
export class AuthService {
  private baseUrl = 'api/users';

  constructor(private http: HttpClient) {}

  login(username: string, password: string): Observable<LoginResponse> {
    const request: LoginRequest = { username, password };
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, request);
  }
  storeTokens(response: LoginResponse): void {
    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('refreshToken', response.refreshToken);
  }

  getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  refreshToken(token: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/refresh-token/${token}`, {});
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  updateAccessToken(newToken: string): void {
    localStorage.setItem('accessToken', newToken);
  }
}