import { Component, signal } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { RouterOutlet } from '@angular/router';
import { Header } from './components/layout/header/header';
import { Footer } from './components/layout/footer/footer';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, Header, Footer],
  template: `<router-outlet></router-outlet>`,
  styleUrl: './app.scss'
})

export class App {
  protected readonly title = signal('EvevtHub');
  showLayout = true;

  constructor(private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        const noLayoutRoutes = ['/login', '/register'];
        this.showLayout = !noLayoutRoutes.includes(event.url);
      }
    });
  }

}
