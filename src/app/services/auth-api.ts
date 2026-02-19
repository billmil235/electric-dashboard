import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserRegistration } from '../models/user-registration.model';

@Injectable({
  providedIn: 'root',
})
export class AuthApi {

  constructor(private http: HttpClient) {}

  register(user: UserRegistration): Observable<void> {
    return this.http.post<void>(`api/users/register`, user);
  }
}
