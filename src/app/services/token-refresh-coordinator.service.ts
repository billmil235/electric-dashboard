import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TokenRefreshCoordinatorService {
  private isRefreshingSubject = new BehaviorSubject<boolean>(false);
  public isRefreshing$ = this.isRefreshingSubject.asObservable();

  private refreshTokenSubject = new BehaviorSubject<string | null>(null);
  public refreshToken$ = this.refreshTokenSubject.asObservable();

  setIsRefreshing(isRefreshing: boolean): void {
    this.isRefreshingSubject.next(isRefreshing);
  }

  setRefreshToken(token: string | null): void {
    this.refreshTokenSubject.next(token);
  }

  getIsRefreshing(): boolean {
    return this.isRefreshingSubject.value;
  }
}