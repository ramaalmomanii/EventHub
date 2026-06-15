import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { importProvidersFrom } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { Register } from './app/components/register/register'
import { Login } from './app/components/login/login';
import { signal } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http'; 
import { authInterceptor } from './app/interceptors/auth-interceptor'; 

bootstrapApplication(App, {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])) 
  ]
}).catch(err => console.error(err));
