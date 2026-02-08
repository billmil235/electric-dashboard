import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject, from } from 'rxjs';
import { catchError, filter, take, switchMap, finalize } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { TokenRefreshService } from '../services/token-refresh.service';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private authService: AuthService, private router: Router) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && error.error?.message === 'Unauthorized') {
          const refreshToken = this.authService.getRefreshToken();
          if (!refreshToken) {
            this.authService.logout();
            return throwError(() => error);
          }

          if (this.isRefreshing) {
            return this.refreshTokenSubject.pipe(
              filter(token => token !== null),
              take(1),
              switchMap(() => next.handle(this.addToken(request, this.authService.getAccessToken()!)))
            );
          }

          this.isRefreshing = true;
          this.refreshTokenSubject.next(null);

          return from(this.authService.refreshToken(refreshToken).toPromise())
            .pipe(
              switchMap((token: any) => {
                this.isRefreshing = false;
                this.refreshTokenSubject.next(token.accessToken);
                this.authService.updateAccessToken(token.accessToken);
                return next.handle(this.addToken(request, token.accessToken));
              }),
              catchError((err) => {
                this.isRefreshing = false;
                this.refreshTokenSubject.next(null);
                this.authService.logout();
                this.router.navigate(['/']);
                return throwError(() => err);
              }),
              finalize(() => {
                this.isRefreshing = false;
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