import { Routes } from '@angular/router';
import { Landing } from './pages/landing/landing';
import { Register } from './pages/register/register';
import { Dashboard } from './pages/dashboard/dashboard';
import { BillingInfo } from './pages/billing-info/billing-info';
import { ProfileEdit } from './pages/profile-edit/profile-edit';
import { authGuard } from './guards/auth.guard';
import { Reports } from './pages/reports/reports';

export const routes: Routes = [
  { path: '', component: Landing },
  { path: 'register', component: Register },
  { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
  { path: 'dashboard/profile', component: ProfileEdit, canActivate: [authGuard] },
  { path: 'dashboard/billing', component: BillingInfo, canActivate: [authGuard] },
  { path: 'dashboard/billing/:addressGuid/:billGuid', component: BillingInfo, canActivate: [authGuard] },
  { path: 'reports/overview', component: Reports, canActivate: [authGuard] },
  { path: '**', redirectTo: '/' }
]
