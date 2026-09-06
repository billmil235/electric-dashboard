import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { CacheService } from '../../services/cache.service';

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

  private authService = inject(AuthService);
  private router = inject(Router);
  private cacheService = inject(CacheService);

  login() {
    this.errorMessage.set('');
    this.authService.login(this.username, this.password).subscribe({
      next: () => {
        // Invalidate all caches after successful login
        this.cacheService.invalidateAllCaches();
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