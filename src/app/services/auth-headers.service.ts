import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AuthHeadersService {
  getAuthHeaders(isFormData?: boolean): HttpHeaders {
    const token = localStorage.getItem('accessToken')
    const headers: { [key: string]: string } = {}
    
    if (token) {
      headers['Authorization'] = `Bearer ${token}`
    }
    
    if (!isFormData) {
      headers['Content-Type'] = 'application/json'
    }
    
    return new HttpHeaders(headers)
  }
}