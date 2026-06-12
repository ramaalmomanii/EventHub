import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const roleGuard: CanActivateFn = (route) => {
  const router = inject(Router);
  const userStr = localStorage.getItem('user');

  if (!userStr) {
    router.navigate(['/login']);
    return false;
  }

  const user = JSON.parse(userStr);
  const requiredRoles: string[] = route.data?.['roles'] ?? [];

  if (requiredRoles.length === 0 || requiredRoles.includes(user.role)) {
    return true;
  }

  router.navigate(['/dashboard']);
  return false;
};
