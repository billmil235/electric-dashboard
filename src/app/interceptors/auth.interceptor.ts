import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, filter, take, switchMap, finalize } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { TokenRefreshCoordinatorService } from '../services/token-refresh-coordinator.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const tokenRefreshCoordinator = inject(TokenRefreshCoordinatorService);

  const addToken = (request: HttpRequest<unknown>, token: string): HttpRequest<unknown> => {
    return request.clone({
      headers: request.headers.set('Authorization', 'Bearer ' + token)
    });
  };

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && error.error?.message === 'Unauthorized') {
        console.log('401 Unauthorized error caught, attempting to refresh token');
        const refreshToken = authService.getRefreshToken();
        if (!refreshToken) {
          authService.logout();
          return throwError(() => error);
        }

        if (tokenRefreshCoordinator.getIsRefreshing()) {
          console.log('Waiting for existing refresh to complete...');
          return tokenRefreshCoordinator.refreshToken$.pipe(
            filter(token => token !== null),
            take(1),
            switchMap((token) => next(addToken(req, token!)))
          );
        }

        console.log('Starting manual token refresh');
        tokenRefreshCoordinator.setIsRefreshing(true);
        tokenRefreshCoordinator.setRefreshToken(null);

        return authService.refreshToken(refreshToken).pipe(
          switchMap((token: { accessToken: string }) => {
            console.log('Manual token refresh successful');
            tokenRefreshCoordinator.setIsRefreshing(false);
            tokenRefreshCoordinator.setRefreshToken(token.accessToken);
            authService.updateAccessToken(token.accessToken);
            console.log('New access token stored in localStorage');
            return next(addToken(req, token.accessToken));
          }),
          catchError((err) => {
            console.error('Manual token refresh failed:', err);
            tokenRefreshCoordinator.setIsRefreshing(false);
            tokenRefreshCoordinator.setRefreshToken(null);
            authService.logout();
            router.navigate(['/']);
            return throwError(() => err);
          }),
          finalize(() => {
            tokenRefreshCoordinator.setIsRefreshing(false);
          })
        );
      }

      return throwError(() => error);
    })
  );
};