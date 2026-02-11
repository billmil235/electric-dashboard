import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-logged-in-layout',
  templateUrl: './logged-in-layout.html',
  styleUrl: './logged-in-layout.css',
  imports: [RouterLink]
})
export class LoggedInLayout {
  private readonly router = inject(Router);

  logout() {
  }

  navigateToDashboard() {
    this.router.navigate(['/dashboard']);
  }
}
