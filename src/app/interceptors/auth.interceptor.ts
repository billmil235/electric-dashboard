import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject, from } from 'rxjs';
import { catchError, filter, take, switchMap, finalize } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { TokenRefreshCoordinatorService } from '../services/token-refresh-coordinator.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    private authService: AuthService, 
    private router: Router,
    private tokenRefreshCoordinator: TokenRefreshCoordinatorService
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && error.error?.message === 'Unauthorized') {
          console.log('401 Unauthorized error caught, attempting to refresh token');
          const refreshToken = this.authService.getRefreshToken();
          if (!refreshToken) {
            this.authService.logout();
            return throwError(() => error);
          }

          // Wait if a refresh is already in progress (shared state)
          if (this.tokenRefreshCoordinator.getIsRefreshing()) {
            console.log('Waiting for existing refresh to complete...');
            return this.refreshTokenSubject.pipe(
              filter(token => token !== null),
              take(1),
              switchMap(() => next.handle(this.addToken(request, this.authService.getAccessToken()!)))
            );
          }

          // Start refreshing the token
          console.log('Starting manual token refresh');
          this.tokenRefreshCoordinator.setIsRefreshing(true);
          this.refreshTokenSubject.next(null);

return from(this.authService.refreshToken(refreshToken).toPromise())
              .pipe(
                switchMap((token: any) => {
                  console.log('Manual token refresh successful');
                  this.tokenRefreshCoordinator.setIsRefreshing(false);
                  this.refreshTokenSubject.next(token.accessToken);
                  // Make sure the new access token is stored in localStorage  
                  this.authService.updateAccessToken(token.accessToken);
                  console.log('New access token stored in localStorage');
                  // Verify the token was stored correctly
                  const storedToken = this.authService.getAccessToken();
                  console.log('Verified stored token:', storedToken);
                  return next.handle(this.addToken(request, token.accessToken));
                }),
              catchError((err) => {
                console.error('Manual token refresh failed:', err);
                this.tokenRefreshCoordinator.setIsRefreshing(false);
                this.refreshTokenSubject.next(null);
                this.authService.logout();
                this.router.navigate(['/']);
                return throwError(() => err);
              }),
              finalize(() => {
                this.tokenRefreshCoordinator.setIsRefreshing(false);
              })
            );
        }

        return throwError(() => error);
      })
    );
  }

  private addToken(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      headers: request.headers.set('Authorization', 'Bearer ' + token)
    });
  }
}