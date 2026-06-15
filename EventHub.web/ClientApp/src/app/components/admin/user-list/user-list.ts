import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../services/user';
import { User, AdminCreateUserDto, AdminUpdateUserDto } from '../../../models/user';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-list.html',
  styleUrl: './user-list.scss'
})
export class UserList implements OnInit {
  private userService = inject(UserService);

  users: User[] = [];
  isLoading = false;
  showModal = false;
  isEditMode = false;
  selectedUser: User | null = null;

  form: AdminCreateUserDto & { id?: number } = {
    fullName: '',
    email: '',
    password: '',
    role: 'Attendee'
  };

  roles = ['Admin', 'Organizer', 'Attendee'];
  statuses = ['Active', 'Inactive'];

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.isLoading = true;
    this.userService.getAll().subscribe({
      next: users => { this.users = users; this.isLoading = false; },
      error: () => this.isLoading = false
    });
  }

  openAddModal() {
    this.isEditMode = false;
    this.form = { fullName: '', email: '', password: '', role: 'Attendee' };
    this.showModal = true;
  }

  openEditModal(user: User) {
    this.isEditMode = true;
    this.selectedUser = user;
    this.form = {
      id: user.id,
      fullName: user.fullName,
      email: user.email,
      password: '',
      role: user.role
    };
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.selectedUser = null;
  }

  saveUser() {
    if (this.isEditMode && this.selectedUser) {
      const dto: AdminUpdateUserDto = {
        fullName: this.form.fullName,
        role: this.form.role,
        status: (this.selectedUser as any).status ?? 'Active'
      };
      this.userService.updateUser(this.selectedUser.id, dto).subscribe({
        next: () => { this.loadUsers(); this.closeModal(); },
        error: err => alert(err.error?.error ?? 'Error updating user')
      });
    } else {
      this.userService.createUser(this.form).subscribe({
        next: () => { this.loadUsers(); this.closeModal(); },
        error: err => alert(err.error?.error ?? 'Error creating user')
      });
    }
  }

  updateStatus(user: User, status: string) {
    const dto: AdminUpdateUserDto = {
      fullName: user.fullName,
      role: user.role,
      status
    };
    this.userService.updateUser(user.id, dto).subscribe({
      next: (updated) => {
        const index = this.users.findIndex(u => u.id === user.id);
        if (index !== -1) this.users[index] = updated;
      },
      error: err => alert(err.error?.error ?? 'Error updating status')
    });
  }

  deleteUser(user: User) {
    if (!confirm(`Delete ${user.fullName}?`)) return;
    this.userService.deleteUser(user.id).subscribe({
      next: () => this.loadUsers()
    });
  }

  getRoleBadgeClass(role: string): string {
    return { Admin: 'badge-admin', Organizer: 'badge-organizer', Attendee: 'badge-attendee' }[role] ?? '';
  }

  getStatusClass(status: string): string {
    return status === 'Active' ? 'status-active' : 'status-inactive';
  }
}
