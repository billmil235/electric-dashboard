import { Routes } from '@angular/router';
import { Landing } from './pages/landing/landing';
import { Register } from './pages/register/register';
import { Dashboard } from './pages/dashboard/dashboard';
import { BillingInfo } from './pages/billing-info/billing-info';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: Landing },
  { path: 'register', component: Register },
  { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
  { path: 'dashboard/billing', component: BillingInfo, canActivate: [authGuard] },
  { path: 'reports/overview', component: Dashboard, canActivate: [authGuard] },
  { path: '**', redirectTo: '/' }
]
