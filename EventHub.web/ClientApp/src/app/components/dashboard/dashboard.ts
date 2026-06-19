import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { EventService } from '../../services/event';
import { PaymentService } from '../../services/payment';
import { UserService } from '../../services/user';
import { RegistrationService } from '../../services/registration';
import { TicketService } from '../../services/ticket';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  private authService = inject(AuthService);
  private eventService = inject(EventService);
  private paymentService = inject(PaymentService);
  private userService = inject(UserService);
  private registrationService = inject(RegistrationService);
  private ticketService = inject(TicketService);
  private router = inject(Router);

  get isAdmin() { return this.authService.isAdmin(); }
  get isOrganizer() { return this.authService.isOrganizer(); }
  get isAttendee() { return this.authService.isAttendee(); }
  get currentUser() { return this.authService.getCurrentUser(); }

  // Admin stats
  totalUsers = 0;
  totalEvents = 0;
  totalPayments = 0;
  totalRevenue = 0;

  // Organizer stats
  myEvents: any[] = [];
  myEventsRevenue = 0;
  myEventsTotalSeats = 0;
  myEventsBookedSeats = 0;

  // Attendee stats
  myRegistrations: any[] = [];
  myTickets: any[] = [];
  myPayments: any[] = [];
  myTotalSpent = 0;

  loading = false;

  ngOnInit() {
    if (!this.authService.getCurrentUser()) {
      this.authService.getMyProfile().subscribe({
        next: () => this.loadData(),
        error: () => this.loadData()
      });
    } else {
      this.loadData();
    }
  }

  loadData() {
    if (this.isAdmin) this.loadAdminData();
    else if (this.isOrganizer) this.loadOrganizerData();
    else this.loadAttendeeData();
  }

  loadAdminData() {
    this.loading = true;
    this.userService.getAll().subscribe({
      next: (users) => this.totalUsers = users.length
    });
    this.eventService.getAll().subscribe({
      next: (events) => this.totalEvents = events.length
    });
    this.paymentService.getAll().subscribe({
      next: (payments) => {
        this.totalPayments = payments.length;
        this.totalRevenue = payments
          .filter(p => p.status === 'Paid')
          .reduce((sum, p) => sum + p.amount, 0);
        this.loading = false;
      }
    });
  }


  loadOrganizerData() {
    this.loading = true;
    this.eventService.getMyEvents().subscribe({
      next: (events) => {
        this.myEvents = events;
        this.myEventsTotalSeats = events.reduce((sum, e) => sum + e.capacity, 0);
        this.myEventsBookedSeats = events.reduce((sum, e) => sum + (e.capacity - e.availableSeats), 0);
        this.loading = false;
      }
    });
  }

  loadAttendeeData() {
    this.loading = true;
    this.registrationService.getMyRegistrations().subscribe({
      next: (regs) => this.myRegistrations = regs
    });
    this.ticketService.getMyTickets().subscribe({
      next: (tickets) => this.myTickets = tickets
    });
    this.paymentService.getMyPayments().subscribe({
      next: (payments) => {
        this.myPayments = payments;
        this.myTotalSpent = payments
          .filter(p => p.status === 'Paid')
          .reduce((sum, p) => sum + p.amount, 0);
        this.loading = false;
      }
    });
  }

  navigate(path: string) {
    this.router.navigate([path]);
  }

  getEventStatusCounts() {
    const counts: Record<string, number> = {};
    this.myEvents.forEach(e => {
      counts[e.status] = (counts[e.status] ?? 0) + 1;
    });
    return counts;
  }

  getOccupancyRate(): number {
    if (this.myEventsTotalSeats === 0) return 0;
    return Math.round((this.myEventsBookedSeats / this.myEventsTotalSeats) * 100);
  }
}
