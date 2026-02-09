import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TokenRefreshCoordinatorService {
  private isRefreshingSubject = new BehaviorSubject<boolean>(false);
  public isRefreshing$ = this.isRefreshingSubject.asObservable();

  setIsRefreshing(isRefreshing: boolean): void {
    this.isRefreshingSubject.next(isRefreshing);
  }

  getIsRefreshing(): boolean {
    return this.isRefreshingSubject.value;
  }
}