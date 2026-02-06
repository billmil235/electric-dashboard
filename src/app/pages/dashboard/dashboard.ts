import { Component, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  username = signal('User');

  constructor(private router: Router) {}

  logout() {
    localStorage.removeItem('accessToken');
    this.router.navigate(['/']);
  }
}