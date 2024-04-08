import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { SpinnerInterceptor } from './services/interceptors/spinner.interceptor';
import { JwtInterceptor } from './services/interceptors/jwt.interceptor';
import { PopupInterceptor } from './services/interceptors/popup.interceptor';

// order matters
const interceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: SpinnerInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: PopupInterceptor, multi: true },
];

export const appConfig: ApplicationConfig = {
  providers: [
    interceptorProviders,
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi())
  ],
};
