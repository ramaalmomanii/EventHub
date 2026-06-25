import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { RegistrationService } from '../../../services/registration';
import { Registration } from '../../../models/registration';

@Component({
  selector: 'app-my-registrations',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-registrations.html',
  styleUrl: './my-registrations.scss'
})
export class MyRegistrations implements OnInit {
  private registrationService = inject(RegistrationService);
  router = inject(Router);

  registrations: Registration[] = [];
  loading = false;
  error = '';
  cancellingId: number | null = null;
  actionError = '';

  ngOnInit() {
    this.loadRegistrations();
  }

  get visibleRegistrations(): Registration[] {
    const now = Date.now();
    return this.registrations.filter(reg => {
      const eventEndTime = reg.eventEndDate ? new Date(reg.eventEndDate).getTime() : Number.POSITIVE_INFINITY;
      return reg.eventStatus !== 'Inactive' && eventEndTime > now;
    });
  }

  loadRegistrations() {
    this.loading = true;
    this.error = '';
    this.registrationService.getMyRegistrations().subscribe({
      next: (data) => {
        this.registrations = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load your registrations.';
        this.loading = false;
      }
    });
  }

  goToEvent(eventId: number) {
    this.router.navigate(['/events', eventId]);
  }

  cancelRegistration(reg: Registration) {
    if (reg.status !== 'Confirmed') return;
    if (!confirm(`Cancel registration for "${reg.eventTitle}"?`)) return;

    this.cancellingId = reg.id;
    this.actionError = '';

    this.registrationService.cancel(reg.id).subscribe({
      next: () => {
        reg.status = 'Cancelled';
        this.cancellingId = null;
      },
      error: (err) => {
        this.actionError = err?.error?.message ?? 'Failed to cancel registration.';
        this.cancellingId = null;
      }
    });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Confirmed': 'status-confirmed',
      'Cancelled': 'status-cancelled',
      'Pending': 'status-pending'
    };
    return map[status] ?? 'status-pending';
  }
}
