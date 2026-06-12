import { Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard';
import { roleGuard } from './guards/role-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  // Auth — public
  {
    path: 'login',
    loadComponent: () => import('./components/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    loadComponent: () => import('./components/register/register').then(m => m.Register)
  },

  // Protected
  {
    path: '',
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./components/dashboard/dashboard').then(m => m.Dashboard)
      },
      {
        path: 'profile',
        loadComponent: () => import('./components/profile/profile').then(m => m.Profile)
      },
      {
        path: 'events',
        loadComponent: () => import('./components/event-list/event-list').then(m => m.EventList)
      },
      {
        path: 'events/:id',
        loadComponent: () => import('./components/events/event-detail/event-detail').then(m => m.EventDetail)
      },
      {
        path: 'my-tickets',
        loadComponent: () => import('./components/tickets/my-tickets/my-tickets').then(m => m.MyTickets)
      },
      {
        path: 'my-registrations',
        loadComponent: () => import('./components/registrations/my-registrations/my-registrations').then(m => m.MyRegistrations)
      },

      // Organizer + Admin
      {
        path: 'events/create',
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'Organizer'] },
        loadComponent: () => import('./components/events/event-form/event-form').then(m => m.EventForm)
      },

      // Admin only
      {
        path: 'admin/users',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () => import('./components/admin/user-list/user-list').then(m => m.UserList)
      },
      {
        path: 'admin/categories',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () => import('./components/categories/category-list/category-list').then(m => m.CategoryList)
      },
      {
        path: 'admin/payments',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () => import('./components/payments/payments').then(m => m.Payments)
      },
    ]
  },

  { path: '**', redirectTo: 'login' }
];
