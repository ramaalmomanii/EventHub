import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Ticket } from '../../../models/ticket';

@Component({
  selector: 'app-ticket-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ticket-card.html',
  styleUrl: './ticket-card.scss'
})
export class TicketCard {
  @Input({ required: true }) ticket!: Ticket;
  @Output() download = new EventEmitter<number>();

  downloading = false;

  onDownload() {
    this.downloading = true;
    this.download.emit(this.ticket.id);
  }

  resetDownloading() {
    this.downloading = false;
  }
}
