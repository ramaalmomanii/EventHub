import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../../services/event';
import { CategoryService } from '../../../services/category';
import { Category } from '../../../models/category';
import { EventCreateDto, EventUpdateDto } from '../../../models/event';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './event-form.html',
  styleUrl: './event-form.scss'
})
export class EventForm implements OnInit {
  private eventService = inject(EventService);
  private categoryService = inject(CategoryService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  eventId: number | null = null;
  isEditMode = false;
  loading = false;
  saving = false;
  error = '';
  categories: Category[] = [];

  form = {
    title: '',
    description: '',
    categoryId: 0,
    startDate: '',
    endDate: '',
    location: '',
    price: 0,
    capacity: 0
  };

  ngOnInit() {
    this.loadCategories();
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.eventId = +id;
      this.isEditMode = true;
      this.loadEvent(this.eventId);
    }
  }

  loadCategories() {
    this.categoryService.getAll().subscribe({
      next: (data) => this.categories = data,
      error: () => this.error = 'Failed to load categories.'
    });
  }

  loadEvent(id: number) {
    this.loading = true;
    this.eventService.getById(id).subscribe({
      next: (event) => {
        this.form = {
          title: event.title,
          description: event.description,
          categoryId: event.categoryId,
          startDate: this.toDatetimeLocal(event.startDate),
          endDate: this.toDatetimeLocal(event.endDate),
          location: event.location,
          price: event.price,
          capacity: event.capacity
        };
        this.loading = false;
      },
      error: () => { this.error = 'Failed to load event.'; this.loading = false; }
    });
  }

  toDatetimeLocal(dateStr: string): string {
    const d = new Date(dateStr);
    const offset = d.getTimezoneOffset();
    const local = new Date(d.getTime() - offset * 60000);
    return local.toISOString().slice(0, 16);
  }

  successMessage = '';

  submit() {
    if (!this.form.title || !this.form.categoryId || !this.form.startDate || !this.form.endDate) {
      this.error = 'Please fill in all required fields.';
      return;
    }
    if (new Date(this.form.startDate) >= new Date(this.form.endDate)) {
      this.error = 'End date must be after start date.';
      return;
    }

    this.saving = true;
    this.error = '';

    if (this.isEditMode && this.eventId) {
      const dto: EventUpdateDto = {
        title: this.form.title,
        description: this.form.description,
        startDate: this.form.startDate,
        endDate: this.form.endDate,
        location: this.form.location,
        price: this.form.price,
        capacity: this.form.capacity
      };
      this.eventService.update(this.eventId, dto).subscribe({
        next: (updated) => {
          this.saving = false;
          this.showSuccess('Event updated successfully!', () => {
            this.router.navigate(['/events', updated.id]);
          });
        },
        error: (err) => { this.error = err?.error?.message ?? 'Failed to save.'; this.saving = false; }
      });
    } else {
      const dto: EventCreateDto = { ...this.form };
      this.eventService.create(dto).subscribe({
        next: (created) => {
          this.saving = false;
          this.showSuccess('Event created successfully!', () => {
            this.router.navigate(['/events', created.id]);
          });
        },
        error: (err) => { this.error = err?.error?.message ?? 'Failed to save.'; this.saving = false; }
      });
    }
  }

  showSuccess(message: string, onClose: () => void) {
    this.successMessage = message;
    setTimeout(() => {
      this.successMessage = '';
      onClose();
    }, 2000);
  }
  cancel() {
    if (this.isEditMode && this.eventId) {
      //this.router.navigate(['/events', this.eventId]);
      this.router.navigate(['/events']);
    } else {
      this.router.navigate(['/events']);
    }
  }
}
