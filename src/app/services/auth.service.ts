import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { JwtTokenService } from './jwt-token.service';

interface LoginRequest {
  username: string;
  password: string;
}

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
  private http = inject(HttpClient);
  private jwtTokenService = inject(JwtTokenService);
  private baseUrl = 'api/users';

  login(username: string, password: string): Observable<LoginResponse> {
    const request: LoginRequest = { username, password };
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, request).pipe(
      tap((response: LoginResponse) => this.jwtTokenService.storeTokens(response))
    );
  }

  getAccessToken(): string | null {
    return this.jwtTokenService.getToken();
  }

  logout(): void {
    this.jwtTokenService.logout();
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

  getDecodedToken(): any {
    const token = this.getAccessToken();
    if (!token) {
      return null;
    }
    return this.jwtTokenService.decodeToken(token);
  }

  isTokenExpired(): boolean {
    return this.jwtTokenService.isTokenExpired();
  }

  getUserId(): string | null {
    return this.jwtTokenService.getUserId();
  }

  getUsername(): string | null {
    return this.jwtTokenService.getUsername();
  }

  getRoles(): string[] {
    return this.jwtTokenService.getRoles();
  }

  hasRole(role: string): boolean {
    return this.jwtTokenService.hasRole(role);
  }
}