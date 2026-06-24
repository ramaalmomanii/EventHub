import { Component, inject, OnInit, QueryList, ViewChildren } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../services/ticket';
import { Ticket } from '../../../models/ticket';
import { TicketCard } from '../ticket-card/ticket-card';

@Component({
  selector: 'app-my-tickets',
  standalone: true,
  imports: [CommonModule, TicketCard],
  templateUrl: './my-tickets.html',
  styleUrl: './my-tickets.scss'
})
export class MyTickets implements OnInit {
  private ticketService = inject(TicketService);

  @ViewChildren(TicketCard) ticketCards!: QueryList<TicketCard>;

  tickets: Ticket[] = [];
  loading = false;
  error = '';
  downloadError = '';

  ngOnInit() {
    this.loadTickets();
  }

  loadTickets() {
    this.loading = true;
    this.error = '';
    this.ticketService.getMyTickets().subscribe({
      next: (data) => {
        this.tickets = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load your tickets.';
        this.loading = false;
      }
    });
  }

  downloadTicket(ticketId: number) {
    this.downloadError = '';
    this.ticketService.downloadPdf(ticketId).subscribe({
      next: (blob) => {
        const ticket = this.tickets.find(t => t.id === ticketId);
        const filename = ticket?.eventTitle
          ? `ticket-${ticket.eventTitle.replace(/\s+/g, '-').toLowerCase()}.pdf`
          : `ticket-${ticketId}.pdf`;

        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;
        link.click();
        window.URL.revokeObjectURL(url);

        const card = this.ticketCards.find(c => c.ticket.id === ticketId);
        card?.resetDownloading();
      },
      error: () => {
        this.downloadError = 'Failed to download ticket PDF.';
        const card = this.ticketCards.find(c => c.ticket.id === ticketId);
        card?.resetDownloading();
      }
    });
  }
}
