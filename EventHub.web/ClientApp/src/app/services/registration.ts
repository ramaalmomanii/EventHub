import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Registration, RegistrationCreateDto } from '../models/registration';

@Injectable({ providedIn: 'root' })
export class RegistrationService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:44370/api/registration';

  register(dto: RegistrationCreateDto): Observable<Registration> {
    return this.http.post<Registration>(this.apiUrl, dto);
  }

  getMyRegistrations(): Observable<Registration[]> {
    return this.http.get<Registration[]>(`${this.apiUrl}/my-registrations`);
  }

  getByEvent(eventId: number): Observable<Registration[]> {
    return this.http.get<Registration[]>(`${this.apiUrl}/event/${eventId}`);
  }

  cancel(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getByUserAndEvent(userId: number, eventId: number): Observable<Registration> {
    return this.http.get<Registration>(`${this.apiUrl}/user/${userId}/event/${eventId}`);
  }
}
