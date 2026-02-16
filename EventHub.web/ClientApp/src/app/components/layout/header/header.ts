import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-header',
  templateUrl: './header.html',
  styleUrls: ['./header.scss']
})
export class Header {
  userMenuItems: MenuItem[];
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
    localStorage.removeItem('token'); // أو حسب كيف مخزن التوكين
    this.router.navigate(['/login']);
    this.dropdownOpen = false;
  }
}
