import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-landing',
  imports: [FormsModule, RouterLink],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing {
  username = '';
  password = '';
  errorMessage = signal('');

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    this.errorMessage.set('');
    this.authService.login(this.username, this.password).subscribe({
      next: (response) => {
        this.authService.storeTokens(response);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        if (err instanceof HttpErrorResponse && err.error?.errorMessage) {
          this.errorMessage.set(err.error.errorMessage);
        } else {
          this.errorMessage.set('Login failed. Please check your credentials and try again.');
        }
      }
    });
  }
}