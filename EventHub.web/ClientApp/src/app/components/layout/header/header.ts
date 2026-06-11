// header.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';

interface NavItem {
  label: string;
  command: () => void;
}

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [],
  templateUrl: './header.html',
  styleUrls: ['./header.scss']
})
export class Header {
  userMenuItems: NavItem[]; 
  dropdownOpen = false;

  constructor(private router: Router) {
    this.userMenuItems = [
      { label: 'My Profile', command: () => this.goToProfile() },
      { label: 'Logout', command: () => this.logout() }
    ];
  }

  toggleDropdown() {
    this.dropdownOpen = !this.dropdownOpen;
  }

  goToProfile() {
    this.router.navigate(['/profile']);
    this.dropdownOpen = false;
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
    this.dropdownOpen = false;
  }
}
