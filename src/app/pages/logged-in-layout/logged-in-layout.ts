import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-logged-in-layout',
  templateUrl: './logged-in-layout.html',
  styleUrl: './logged-in-layout.css',
  imports: [RouterLink]
})
export class LoggedInLayout {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  
  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
