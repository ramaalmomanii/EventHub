import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { EventService } from '../../services/event';
import { AuthService } from '../../services/auth.service';
import { RegistrationService } from '../../services/registration';
import { Event } from '../../models/event';
import { Registration } from '../../models/registration';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './event-list.html',
  styleUrl: './event-list.scss'
})
export class EventList implements OnInit {
  private eventService = inject(EventService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private registrationService = inject(RegistrationService);

  events: Event[] = [];
  loading = false;
  error = '';
  deletingId: number | null = null;
  myRegistrations: Registration[] = [];

  get currentRole() { return this.authService.getRole(); }
  get currentUserId() { return this.authService.getUserId(); }
  get isAdmin() { return this.authService.isAdmin(); }
  get isOrganizer() { return this.authService.isOrganizer(); }
  get canCreate() { return this.isAdmin || this.isOrganizer; }
  get isAttendee() { return this.authService.isAttendee(); }

  canEdit(event: Event): boolean {
    return this.isAdmin || event.organizerId === this.currentUserId;
  }

  canDelete(event: Event): boolean {
    return this.isAdmin || event.organizerId === this.currentUserId;
  }

  ngOnInit() {
    this.loadEvents();
    if (this.isAttendee) this.loadMyRegistrations();
  }

  loadMyRegistrations() {
    this.registrationService.getMyRegistrations().subscribe({
      next: (regs) => this.myRegistrations = regs,
      error: () => { }
    });
  }

  isRegistered(eventId: number): boolean {
    return this.myRegistrations.some(r => r.eventId === eventId && r.status === 'Confirmed');
  }

  registerEvent(event: Event) {
    this.registrationService.register({ eventId: event.id }).subscribe({
      next: (reg) => {
        this.myRegistrations.push(reg);
        event.availableSeats--;
      },
      error: (err) => { this.error = err?.error?.message ?? 'Registration failed.'; }
    });
  }
  loadEvents() {
    this.loading = true;
    this.error = '';
    const source$ = this.isOrganizer
      ? this.eventService.getMyEvents()
      : this.eventService.getAll();

    source$.subscribe({
      next: (data) => { this.events = data; this.loading = false; },
      error: () => { this.error = 'Failed to load events.'; this.loading = false; }
    });
  }

  goToCreate() {
    this.router.navigate(['/events/new']);
  }

  goToEdit(id: number) {
    this.router.navigate(['/events', id, 'edit']);
  }

  goToDetail(id: number) {
    this.router.navigate(['/events', id]);
  }

  deleteEvent(event: Event) {
    if (!confirm(`Delete "${event.title}"?`)) return;
    this.deletingId = event.id;
    this.eventService.delete(event.id).subscribe({
      next: () => {
        this.events = this.events.filter(e => e.id !== event.id);
        this.deletingId = null;
      },
      error: () => { this.error = 'Failed to delete event.'; this.deletingId = null; }
    });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Active': 'status-active',
      'Cancelled': 'status-cancelled',
      'Completed': 'status-completed',
      'Upcoming':'status-upcoming',
      'Draft': 'status-draft'
    };
    return map[status] ?? 'status-draft';
  }
}
