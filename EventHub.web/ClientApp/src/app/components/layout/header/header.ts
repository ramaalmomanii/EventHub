import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';
import { User } from '../../../models/user';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './header.html',
  styleUrls: ['./header.scss']
})
export class Header implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  currentUser: User | null = null;
  dropdownOpen = false;

  ngOnInit() {
    this.authService.getMyProfile().subscribe({
      next: user => this.currentUser = user,
      error: () => { }
    });
  }

  get initials(): string {
    return this.currentUser?.fullName
      ?.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2) ?? '?';
  }

  toggleDropdown() {
    this.dropdownOpen = !this.dropdownOpen;
  }

  closeDropdown() {
    this.dropdownOpen = false;
  }

  goToProfile() {
    this.router.navigate(['/profile']);
    this.closeDropdown();
  }

  logout() {
    this.authService.logout();
    this.closeDropdown();
  }
}
