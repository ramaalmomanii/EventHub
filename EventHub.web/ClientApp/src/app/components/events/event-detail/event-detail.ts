import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../../services/event';
import { AuthService } from '../../../services/auth.service';
import { Event } from '../../../models/event';

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

  event: Event | null = null;
  loading = false;
  error = '';

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
    if (id) this.loadEvent(+id);
  }

  loadEvent(id: number) {
    this.loading = true;
    this.eventService.getById(id).subscribe({
      next: (data) => { this.event = data; this.loading = false; },
      error: () => { this.error = 'Event not found.'; this.loading = false; }
    });
  }

  goToEdit() {
    this.router.navigate(['/events', this.event!.id, 'edit']);
  }

  goBack() {
    this.router.navigate(['/events']);
  }
}
