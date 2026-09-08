import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root',
})
export class JwtTokenService {
  private jwtHelper = new JwtHelperService();

  decodeToken(token: string) {
    try {
      return this.jwtHelper.decodeToken(token);
    } catch {
      return null;
    }
  }

  getTokenExpirationDate(token: string): Date | null {
    try {
      const expiration = this.jwtHelper.getTokenExpirationDate(token);
      if (!expiration) {
        return null;
      }
      return expiration;
    } catch {
      return null;
    }
  }

  isTokenExpired(token?: string): boolean {
    const tokenToCheck = token || this.getToken();
    if (!tokenToCheck) {
      return true;
    }
    return this.jwtHelper.isTokenExpired(tokenToCheck);
  }

  getToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  getUserId(): string | null {
    const token = this.getToken();
    if (!token) {
      return null;
    }
    const decoded = this.decodeToken(token);
    return decoded?.sub || decoded?.userId || null;
  }

  getUsername(): string | null {
    const token = this.getToken();
    if (!token) {
      return null;
    }
    const decoded = this.decodeToken(token);
    return decoded?.preferred_username || decoded?.username || decoded?.name || null;
  }

  getRoles(): string[] {
    const token = this.getToken();
    if (!token) {
      return [];
    }
    const decoded = this.decodeToken(token);
    return decoded?.realm_access?.roles || decoded?.roles || [];
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  hasRefreshToken(): boolean {
    return this.getRefreshToken() !== null;
  }

  hasRole(role: string): boolean {
    return this.getRoles().includes(role);
  }

  storeTokens(tokens: { accessToken: string; refreshToken: string }): void {
    localStorage.setItem('accessToken', tokens.accessToken);
    localStorage.setItem('refreshToken', tokens.refreshToken);
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }
}