import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminCreateUserDto, AdminUpdateUserDto, User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:44370/api/user';

  getAll(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl);
  }

  getById(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`);
  }

  getByRole(role: string): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/role/${role}`);
  }

  updateProfile(dto: { fullName: string }): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/me`, dto);
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  createUser(dto: AdminCreateUserDto): Observable<User> {
    return this.http.post<User>(this.apiUrl, dto);
  }

  updateUser(id: number, dto: AdminUpdateUserDto): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/${id}`, dto);
  }
}
