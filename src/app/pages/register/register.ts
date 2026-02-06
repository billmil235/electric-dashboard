import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Api } from '../../services/api';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  email = '';
  password = '';
  firstName = '';
  lastName = '';
  dob = '';

  constructor(private api: Api, private router: Router) {}

  register() {
    const dobDate = new Date(this.dob);
    this.api
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
        error: (err) => {
          alert('Registration failed: ' + err.message);
        },
      });
  }
}