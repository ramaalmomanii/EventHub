import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AiProvider, EventSummary } from '../models/event-summary';

@Injectable({ providedIn: 'root' })
export class EventSummaryService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:44370/api/event';

  getSummary(eventId: number, provider: AiProvider): Observable<EventSummary> {
    return this.http.get<EventSummary>(`${this.apiUrl}/${eventId}/summary`, {
      params: { provider }
    });
  }
}
