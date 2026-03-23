import { Injectable } from '@angular/core';

interface CacheInvalidator {
  invalidateCache: () => void;
}

@Injectable({
  providedIn: 'root',
})
export class CacheService {
  private invalidators: CacheInvalidator[] = [];

  registerCacheInvalidator(invalidator: CacheInvalidator): void {
    this.invalidators.push(invalidator);
  }

  invalidateAllCaches(): void {
    this.invalidators.forEach((invalidator) => {
      try {
        invalidator.invalidateCache();
      } catch (error) {
        console.error('Error invalidating cache:', error);
      }
    });
  }
}

export type { CacheInvalidator };
