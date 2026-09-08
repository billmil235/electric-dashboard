import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Subject, takeUntil } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { inject } from '@angular/core';
import { AuthApi } from '../../services/auth-api';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register implements OnInit, OnDestroy {
  email = '';
  password = '';
  confirmPassword = '';
  firstName = '';
  lastName = '';
  dob = '';
  registrationError = '';
  isSubmitting = false;
  emailAlreadyExists = false;
  isCheckingEmail = false;

  private emailCheck$ = new Subject<string>();

  private authApi = inject(AuthApi);
  private router = inject(Router);

  ngOnInit(): void {
    this.emailCheck$
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe((email) => {
        if (!email || email.length < 3) {
          this.emailAlreadyExists = false;
          return;
        }
        this.isCheckingEmail = true;
        this.authApi
          .checkEmailExists(email)
          .subscribe({
            next: (exists: boolean) => {
              this.emailAlreadyExists = exists;
              this.isCheckingEmail = false;
            },
            error: () => {
              this.emailAlreadyExists = false;
              this.isCheckingEmail = false;
            },
          });
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.emailCheck$.complete();
  }

  private destroy$ = new Subject<void>();

  get passwordsDoNotMatch(): boolean {
    return this.password !== '' && this.confirmPassword !== '' && this.password !== this.confirmPassword;
  }

  register() {
    this.registrationError = '';
    if (this.password !== this.confirmPassword) {
      return;
    }
    const dobDate = new Date(this.dob);
    this.isSubmitting = true;
    this.authApi
      .register({
        emailAddress: this.email,
        password: this.password,
        dateOfBirth: dobDate,
        firstName: this.firstName,
        lastName: this.lastName,
      })
      .subscribe({
        next: () => {
          this.router.navigate(['/']);
        },
        error: (err: unknown) => {
          this.isSubmitting = false;
          this.registrationError = err instanceof HttpErrorResponse
            ? `Registration failed: ${err.error ?? err.message}`
            : 'Registration failed. Please try again.';
        },
      });
  }

  onEmailChange(email: string): void {
    this.emailCheck$.next(email);
  }
}