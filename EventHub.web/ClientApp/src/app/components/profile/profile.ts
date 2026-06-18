import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { User } from '../../models/user';



@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class Profile implements OnInit {
  private authService = inject(AuthService);

  user = signal<User | null>(null);
  loading = signal(false);
  errorMsg = signal('');
  editMode = false;
  saving = false;
  successMsg = '';

  editForm = {
    fullName: '',
    email: '',
    phone: ''
  };

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.loading.set(true);
    this.authService.getMyProfile().subscribe({
      next: (res) => { this.user.set(res); this.loading.set(false); },
      error: () => { this.errorMsg.set('Failed to load profile.'); this.loading.set(false); }
    });
  }

  get initials(): string {
    return this.user()?.fullName
      ?.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2) ?? '?';
  }

  openEdit() {
    const u = this.user();
    if (!u) return;
    this.editForm = {
      fullName: u.fullName ?? '',
      email: u.email ?? '',
      phone: (u as any).phone ?? ''
    };
    this.editMode = true;
    this.successMsg = '';
    this.errorMsg.set('');
  }

  cancelEdit() {
    this.editMode = false;
  }

  saveEdit() {
    this.saving = true;
    this.errorMsg.set('');
    this.authService.updateProfile(this.editForm).subscribe({
      next: (updated) => {
        this.user.set(updated);
        this.saving = false;
        this.editMode = false;
        this.showSuccess('Profile updated successfully!');
      },
      error: (err) => {
        this.errorMsg.set(err?.error?.message ?? 'Failed to update profile.');
        this.saving = false;
      }
    });
  }

  showSuccess(msg: string) {
    this.successMsg = msg;
    setTimeout(() => this.successMsg = '', 3000);
  }
}
