import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { interval, take, catchError, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TokenRefreshService {
  private refreshInterval: any;
  private isRefreshing = false;

  constructor(private authService: AuthService) {}

  startAutoRefresh(): void {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }

    const refreshToken = this.authService.getRefreshToken();
    if (!refreshToken) {
      return;
    }

    this.refreshInterval = setInterval(() => {
      console.log('Attempting to refresh token...');
      this.attemptRefresh();
    }, 5 * 60 * 1000); // Every 5 minutes (5 * 60 * 1000 milliseconds)
  }

  stopAutoRefresh(): void {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
      this.refreshInterval = null;
    }
  }

  private attemptRefresh(): void {
    if (this.isRefreshing) {
      return;
    }

    const refreshToken = this.authService.getRefreshToken();
    console.log('New refresh token:', refreshToken); // Debug log
    if (!refreshToken) {
      return;
    }

    this.isRefreshing = true;

    this.authService.refreshToken(refreshToken).pipe(
      take(1),
      catchError((error) => {
        console.error('Token refresh failed:', error);
        this.isRefreshing = false;
        return throwError(() => error);
      })
    ).subscribe({
      next: (response) => {
        this.authService.updateAccessToken(response.accessToken);
        this.isRefreshing = false;
      },
      error: (err) => {
        console.error('Failed to refresh token:', err);
        this.isRefreshing = false;
        this.authService.logout();
      }
    });
  }
}