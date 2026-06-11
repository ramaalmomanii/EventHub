import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';


export interface RegisterDto {
  fullName: string;
  email: string;
  password: string;
  role: string;
}
export interface LoginDto {
  email: string;
  password: string;
}
export interface UserProfile {
  fullName: string;
  email: string;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:44370/api/user';
  constructor(private http: HttpClient) { }

  // Register a new user
  register(dto: RegisterDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, dto);

  }
  // Login a user
  login(dto: LoginDto): Observable<{ accessToken: string; refreshToken: string }> {
    return this.http.post<{ accessToken: string; refreshToken: string }>(`${this.apiUrl}/login`, dto);
  }
  // get user profile
  getMyProfile(): Observable<UserProfile> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
    return this.http.get<UserProfile>(`${this.apiUrl}/me`, { headers });
  }

  // get all users >>> admin
  getAllUsers(): Observable<UserProfile[]> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
    return this.http.get<UserProfile[]>(`${this.apiUrl}/user`, { headers });
  }
}
