import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../../services/event';
import { AuthService } from '../../../services/auth.service';
import { Event } from '../../../models/event';
import { RegistrationService } from '../../../services/registration';


@Component({
  selector: 'app-event-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './event-detail.html',
  styleUrl: './event-detail.scss'
})
export class EventDetail implements OnInit {
  private eventService = inject(EventService);
  private authService = inject(AuthService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private registrationService = inject(RegistrationService);

  event: Event | null = null;
  loading = false;
  error = '';

  myRegistration: any = null;
  registering = false;
  registrationError = '';
  registrationSuccess = '';


  get isAdmin() { return this.authService.isAdmin(); }
  get isOrganizer() { return this.authService.isOrganizer(); }
  get currentUserId() { return this.authService.getUserId(); }

  get canEdit(): boolean {
    return this.isAdmin || this.event?.organizerId === this.currentUserId;
  }

  get statusClass(): string {
    const map: Record<string, string> = {
      'Active': 'status-active',
      'Cancelled': 'status-cancelled',
      'Completed': 'status-completed',
      'Draft': 'status-draft'
    };
    return this.event ? (map[this.event.status] ?? 'status-draft') : '';
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadEvent(+id);
      this.loadMyRegistration(+id);
    }
  }

  loadEvent(id: number) {
    this.loading = true;
    this.eventService.getById(id).subscribe({
      next: (data) => { this.event = data; this.loading = false; },
      error: () => { this.error = 'Event not found.'; this.loading = false; }
    });
  }

  loadMyRegistration(eventId: number) {
    const userId = this.authService.getUserId();
    this.registrationService.getByUserAndEvent(userId, eventId).subscribe({
      next: (reg) => this.myRegistration = reg,
      error: () => this.myRegistration = null
    });
  }

  register() {
    if (!this.event) return;
    this.registering = true;
    this.registrationError = '';

    this.registrationService.register({ eventId: this.event.id }).subscribe({
      next: (reg) => {
        this.myRegistration = reg;
        this.registering = false;
        this.registrationSuccess = 'Registered successfully!';
        if (this.event) this.event.availableSeats--;
        setTimeout(() => this.registrationSuccess = '', 3000);
      },
      error: (err) => {
        this.registrationError = err?.error?.message ?? 'Registration failed.';
        this.registering = false;
      }
    });
  }

  cancelRegistration() {
    if (!this.myRegistration) return;
    if (!confirm('Cancel your registration?')) return;
    this.registering = true;

    this.registrationService.cancel(this.myRegistration.id).subscribe({
      next: () => {
        this.myRegistration = null;
        this.registering = false;
        if (this.event) this.event.availableSeats++;
      },
      error: (err) => {
        this.registrationError = err?.error?.message ?? 'Failed to cancel.';
        this.registering = false;
      }
    });
  }

  get isAttendee() { return this.authService.isAttendee(); }
  get isRegistered() { return this.myRegistration?.status === 'Confirmed'; }
  get isCancelled() { return this.myRegistration?.status === 'Cancelled'; }

  goToEdit() {
    this.router.navigate(['/events', this.event!.id, 'edit']);
  }

  goBack() {
    this.router.navigate(['/events']);
  }
}
