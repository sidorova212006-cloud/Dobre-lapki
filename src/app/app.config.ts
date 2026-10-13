import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';

import { routes } from './app.routes';
import {
  ANIMALS_API,
  APPLICATIONS_API,
  AUTH_API,
  FAVORITES_API,
} from './core/tokens/injection-tokens';
import { MockAuthApi } from './core/mock/auth.mock';
import { MockAnimalsApi } from './core/mock/animals.mock';
import { MockFavoritesApi } from './core/mock/favorites.mock';
import { MockApplicationsApi } from './core/mock/applications.mock';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withInMemoryScrolling({ scrollPositionRestoration: 'top' })),

    // TODO(BACKEND): заменить useClass на реальные HTTP-реализации.
    { provide: AUTH_API, useClass: MockAuthApi },
    { provide: ANIMALS_API, useClass: MockAnimalsApi },
    { provide: FAVORITES_API, useClass: MockFavoritesApi },
    { provide: APPLICATIONS_API, useClass: MockApplicationsApi },
  ],
};
