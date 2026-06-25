import { Component, OnInit, inject, HostListener } from '@angular/core';
import { Router, NavigationEnd, RouterLink } from '@angular/router';
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
  isDarkMode = localStorage.getItem('theme') === 'dark';

  constructor() {
    inject(Router).events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.closeDropdown(); 
      }
    });
  }

  ngOnInit() {
    this.applyTheme();

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

  toggleTheme() {
    this.isDarkMode = !this.isDarkMode;
    localStorage.setItem('theme', this.isDarkMode ? 'dark' : 'light');
    this.applyTheme();
  }

  private applyTheme() {
    document.body.classList.toggle('dark-theme', this.isDarkMode);
  }

  closeDropdown() {
    this.dropdownOpen = false;
  }

  goToProfile() {
    this.router.navigate(['/profile']);
    this.closeDropdown();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const clickedElement = event.target as HTMLElement;
    if (!clickedElement.closest('.avatar-wrap')) {
      this.closeDropdown();
    }
  }

  logout() {
    this.authService.logout();
    this.closeDropdown();
  }
}
