import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { LoginDto } from '../models/user';
import {  RegisterDto, TokenResponse, User, UpdateProfileDto } from '../models/user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = 'https://localhost:44370/api/user';

  register(dto: RegisterDto): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/register`, dto);
  }

  login(dto: LoginDto): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.apiUrl}/login`, dto).pipe(
      tap(res => {
        localStorage.setItem('token', res.accessToken);
        localStorage.setItem('refreshToken', res.refreshToken);
      })
    );
  }

  logout(): void {
    localStorage.clear();
    this.router.navigate(['/login']);
  }

  getMyProfile(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/me`).pipe(
      tap(user => localStorage.setItem('user', JSON.stringify(user)))
    );
  }

  updateProfile(dto: UpdateProfileDto): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/me`, dto);
  }

  getCurrentUser(): User | null {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getRole(): string {
    return this.getCurrentUser()?.role ?? '';
  }

  isAdmin(): boolean { return this.getRole() === 'Admin'; }
  isOrganizer(): boolean { return this.getRole() === 'Organizer'; }
  isAttendee(): boolean { return this.getRole() === 'Attendee'; }
}
