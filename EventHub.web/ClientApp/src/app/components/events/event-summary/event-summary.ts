import { Component, Input, OnChanges, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventSummaryService } from '../../../services/event-summary';
import { AiProvider } from '../../../models/event-summary';

@Component({
  selector: 'app-event-summary',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './event-summary.html',
  styleUrl: './event-summary.scss'
})
export class EventSummaryComponent implements OnChanges {
  private summaryService = inject(EventSummaryService);

  @Input({ required: true }) eventId!: number;
  @Input() compact = false;

  summary = '';
  loading = false;
  error = '';
  provider: AiProvider = this.loadProvider();

  ngOnChanges(changes: SimpleChanges) {
    if (changes['eventId'] && this.eventId) {
      this.loadSummary();
    }
  }

  loadProvider(): AiProvider {
    const saved = localStorage.getItem('aiProvider');
    return saved === 'gemini' ? 'gemini' : 'openai';
  }

  switchProvider(provider: AiProvider) {
    if (this.provider === provider) return;
    this.provider = provider;
    localStorage.setItem('aiProvider', provider);
    this.loadSummary();
  }

  loadSummary() {
    this.loading = true;
    this.error = '';
    this.summary = '';

    this.summaryService.getSummary(this.eventId, this.provider).subscribe({
      next: (data) => {
        this.summary = data.summary;
        this.loading = false;
      },
      error: (err) => {
        this.error = err?.error?.message ?? 'Could not generate AI summary.';
        this.loading = false;
      }
    });
  }

  retry() {
    this.loadSummary();
  }
}
