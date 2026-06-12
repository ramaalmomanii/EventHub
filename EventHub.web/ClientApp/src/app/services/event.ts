import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event, EventCreateDto, EventUpdateDto } from '../models/event';

@Injectable({ providedIn: 'root' })
export class EventService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:44370/api/event';

  getAll(): Observable<Event[]> {
    return this.http.get<Event[]>(this.apiUrl);
  }

  getById(id: number): Observable<Event> {
    return this.http.get<Event>(`${this.apiUrl}/${id}`);
  }

  getMyEvents(): Observable<Event[]> {
    return this.http.get<Event[]>(`${this.apiUrl}/my-events`);
  }

  create(dto: EventCreateDto): Observable<Event> {
    return this.http.post<Event>(this.apiUrl, dto);
  }

  update(id: number, dto: EventUpdateDto): Observable<Event> {
    return this.http.put<Event>(`${this.apiUrl}/${id}`, dto);
  }

  updateStatus(id: number, status: string): Observable<Event> {
    return this.http.patch<Event>(`${this.apiUrl}/${id}/status`, status);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
