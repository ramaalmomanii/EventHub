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
    fullName: ''
  };

  showPasswordForm = false;
  savingPassword = false;
  passwordError = '';

  passwordForm = {
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
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

 

  cancelEdit() {
    this.editMode = false;
  }

  openEdit() {
    const u = this.user();
    if (!u) return;
    this.editForm = { fullName: u.fullName ?? '' };
    this.editMode = true;
    this.successMsg = '';
    this.errorMsg.set('');
  }

  saveEdit() {
    if (!this.editForm.fullName.trim()) {
      this.errorMsg.set('Full name is required.');
      return;
    }
    this.saving = true;
    this.errorMsg.set('');
    this.authService.updateProfile({ fullName: this.editForm.fullName }).subscribe({
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

  openPasswordForm() {
    this.showPasswordForm = true;
    this.passwordForm = { currentPassword: '', newPassword: '', confirmPassword: '' };
    this.passwordError = '';
  }

  cancelPasswordForm() {
    this.showPasswordForm = false;
    this.passwordError = '';
  }

  changePassword() {
    if (!this.passwordForm.currentPassword || !this.passwordForm.newPassword) {
      this.passwordError = 'All fields are required.';
      return;
    }
    if (this.passwordForm.newPassword !== this.passwordForm.confirmPassword) {
      this.passwordError = 'Passwords do not match.';
      return;
    }
    if (this.passwordForm.newPassword.length < 6) {
      this.passwordError = 'New password must be at least 6 characters.';
      return;
    }

    this.savingPassword = true;
    this.passwordError = '';

    this.authService.changePassword({
      currentPassword: this.passwordForm.currentPassword,
      newPassword: this.passwordForm.newPassword
    }).subscribe({
      next: () => {
        this.savingPassword = false;
        this.showPasswordForm = false;
        this.showSuccess('Password changed successfully!');
      },
      error: (err) => {
        this.passwordError = err?.error?.message ?? 'Failed to change password.';
        this.savingPassword = false;
      }
    });
  }

  showSuccess(msg: string) {
    this.successMsg = msg;
    setTimeout(() => this.successMsg = '', 3000);
  }
}
