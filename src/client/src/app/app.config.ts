import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';

import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { LoaderInterceptor } from './shared/interceptors/loader.interceptor';

// order matters
const interceptorProviders = [
  //{ provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true },
];

export const appConfig: ApplicationConfig = {
  providers: [
    // interceptorProviders,
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptorsFromDi()),
  ]
};
