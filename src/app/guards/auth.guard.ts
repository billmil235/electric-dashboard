import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);
  const authService = inject(AuthService);

  const token = authService.getAccessToken();
  if (!token) {
    router.navigate(['/']);
    return false;
  }

  if (authService.isTokenExpired()) {
    authService.logout();
    router.navigate(['/']);
    return false;
  }

  return true;
};