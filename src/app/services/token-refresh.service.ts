import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { take, catchError, throwError } from 'rxjs';
import { TokenRefreshCoordinatorService } from './token-refresh-coordinator.service';

@Injectable({
  providedIn: 'root',
})
export class TokenRefreshService {
  private refreshInterval: any;

  constructor(private authService: AuthService, private tokenRefreshCoordinator: TokenRefreshCoordinatorService) {}

  startAutoRefresh(): void {
    if (this.refreshInterval) {
      this.clearInterval();
    }

    // Try to start refresh immediately, regardless of existing interval
    this.scheduleNextRefresh();
  }

  private clearInterval(): void {
    if (this.refreshInterval) {
      if (typeof this.refreshInterval === 'number') {
        clearInterval(this.refreshInterval);
      } else {
        clearTimeout(this.refreshInterval);
      }
      this.refreshInterval = null;
    }
  }

  private scheduleNextRefresh(): void {
    if (this.refreshInterval) {
      this.clearInterval();
    }

    const refreshToken = this.authService.getRefreshToken();
    if (!refreshToken) {
      // If no refresh token, schedule to check again in 30 seconds
      console.log('No refresh token found, will retry in 30 seconds');
      this.refreshInterval = setTimeout(() => {
        this.scheduleNextRefresh();
      }, 30 * 1000);
      return;
    }

    console.log('Starting automatic token refresh timer');
    this.refreshInterval = setInterval(() => {
      // Only proceed if not already refreshing
      if (this.tokenRefreshCoordinator.getIsRefreshing()) {
        console.log('Skipping refresh - another refresh is already in progress');
        return;
      }

      const accessToken = this.authService.getAccessToken();
      if (!accessToken) {
        console.log('No access token found for refresh');
        return;
      }
      
      this.attemptRefresh();
    }, 5 * 60 * 1000); // Every 5 minutes
  }

  stopAutoRefresh(): void {
    this.clearInterval();
    console.log('Stopped automatic token refresh timer');
  }

  private attemptRefresh(): void {
    const refreshToken = this.authService.getRefreshToken();
    if (!refreshToken) {
      console.log('Skipping refresh - no refresh token available');
      return;
    }

    console.log('Attempting automatic token refresh');
    // Set the shared refresh state
    this.tokenRefreshCoordinator.setIsRefreshing(true);

    this.authService.refreshToken(refreshToken).pipe(
      take(1),
      catchError((error) => {
        console.error('Auto token refresh failed:', error);
        this.tokenRefreshCoordinator.setIsRefreshing(false);
        return throwError(() => error);
      })
    ).subscribe({
      next: (response) => {
        console.log('Auto token refresh successful');
        this.authService.updateAccessToken(response.accessToken);
        this.tokenRefreshCoordinator.setIsRefreshing(false);
      },
      error: (err) => {
        console.error('Auto refresh failed and logged out:', err);
        this.tokenRefreshCoordinator.setIsRefreshing(false);
        this.authService.logout();
      }
    });
  }
}