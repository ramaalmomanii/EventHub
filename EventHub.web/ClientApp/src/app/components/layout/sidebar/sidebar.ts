import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';
import { User } from '../../../models/user';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles?: string[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class Sidebar implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  currentUser: User | null = null;

  navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'ti-layout-dashboard', route: '/dashboard' },
    { label: 'Events', icon: 'ti-calendar', route: '/events' },
    { label: 'My Tickets', icon: 'ti-ticket', route: '/my-tickets', roles: ['Attendee'] },
    { label: 'My Registrations', icon: 'ti-clipboard-list', route: '/my-registrations', roles: ['Attendee'] },
    { label: 'My Events', icon: 'ti-calendar-stats', route: '/my-events', roles: ['Organizer'] },
    { label: 'Create Event', icon: 'ti-plus', route: '/events/new', roles: ['Organizer', 'Admin'] },
    { label: 'Users', icon: 'ti-users', route: '/admin/users', roles: ['Admin'] },
    { label: 'Categories', icon: 'ti-tags', route: '/admin/categories', roles: ['Admin'] },
    { label: 'Payments', icon: 'ti-credit-card', route: '/admin/payments', roles: ['Admin'] },
  ];

  ngOnInit() {
    this.authService.getMyProfile().subscribe({
      next: user => this.currentUser = user,
      error: () => { }
    });
  }



  get filteredNavItems(): NavItem[] {
    const role = this.currentUser?.role ?? '';
    return this.navItems.filter(item =>
      !item.roles || item.roles.includes(role)
    );
  }

  // Dashboard + Events + items
  get mainItems(): NavItem[] {
    return this.filteredNavItems.filter(item =>
      !item.roles || item.roles.includes('Attendee')
    );
  }

  // Organizer 
  get organizerItems(): NavItem[] {
    const role = this.currentUser?.role ?? '';
    if (role !== 'Organizer') return [];
    return this.filteredNavItems.filter(i =>
      i.roles?.includes('Organizer') && !i.roles?.includes('Admin')
    );
  }

  // Admin
  get adminItems(): NavItem[] {
    const role = this.currentUser?.role ?? '';
    if (role !== 'Admin') return [];
    return this.filteredNavItems.filter(i => i.roles?.includes('Admin'));
  }

  

  get initials(): string {
    return this.currentUser?.fullName
      ?.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2) ?? '?';
  }

  logout() {
    this.authService.logout();
  }
}
