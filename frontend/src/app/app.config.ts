import { ApplicationConfig, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter, withHashLocation } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    // Activates native change detection and removes zone.js requirements entirely
    provideZonelessChangeDetection(), 
    provideRouter(routes, withHashLocation()),
    provideHttpClient() // Essential to enable your GameService to talk to .NET Core!
  ]
};
