import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthApi } from '../../services/auth-api';
import { NgIf } from "../../../../node_modules/@angular/common/types/_common_module-chunk";

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink, NgIf],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  email = '';
  password = '';
  confirmPassword = '';
  firstName = '';
  lastName = '';
  dob = '';

  constructor(private authApi: AuthApi, private router: Router) {}

  register() {
    if (this.password !== this.confirmPassword) {
      return;
    }
    const dobDate = new Date(this.dob);
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
          alert('Registration failed: ' + (err instanceof Error ? err.message : 'Unknown error'));
        },
      });
  }
}