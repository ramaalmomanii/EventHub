import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaymentService } from '../../services/payment';
import { AuthService } from '../../services/auth.service';
import { Payment } from '../../models/payment';

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './payments.html',
  styleUrl: './payments.scss'
})
export class Payments implements OnInit {
  private paymentService = inject(PaymentService);
  private authService = inject(AuthService);

  payments: Payment[] = [];
  loading = false;
  error = '';

  get isAdmin() { return this.authService.isAdmin(); }

  get pageTitle() {
    return this.isAdmin ? 'All Payments' : 'My Payments';
  }

  get pageSubtitle() {
    return this.isAdmin ? 'All transactions in the system' : 'Your payment history';
  }

  ngOnInit() {
    this.loadPayments();
  }

  loadPayments() {
    this.loading = true;
    this.error = '';

    const source$ = this.isAdmin
      ? this.paymentService.getAll()
      : this.paymentService.getMyPayments();

    source$.subscribe({
      next: (data) => { this.payments = data; this.loading = false; },
      error: () => { this.error = 'Failed to load payments.'; this.loading = false; }
    });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Paid': 'status-completed',
      'Pending': 'status-pending',
      'Failed': 'status-failed',
      'Refunded': 'status-refunded'
    };
    return map[status] ?? 'status-pending';
  }


  getMethodIcon(method: string): string {
    const map: Record<string, string> = {
      'CreditCard': '💳',
      'PayPal': '🅿️',
      'Cash': '💵',
    };
    return map[method] ?? '💳';
  }

  get totalAmount(): number {
    return this.payments
      .filter(p => p.status === 'Paid')
      .reduce((sum, p) => sum + p.amount, 0);
  }
}
