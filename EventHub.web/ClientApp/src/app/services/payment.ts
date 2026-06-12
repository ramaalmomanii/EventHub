import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Payment, PaymentCreateDto } from '../models/payment';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:44370/api/payments';

  processPayment(dto: PaymentCreateDto): Observable<Payment> {
    return this.http.post<Payment>(this.apiUrl, dto);
  }

  getMyPayments(): Observable<Payment[]> {
    return this.http.get<Payment[]>(`${this.apiUrl}/my-payments`);
  }

  getByEvent(eventId: number): Observable<Payment[]> {
    return this.http.get<Payment[]>(`${this.apiUrl}/event/${eventId}`);
  }
}
