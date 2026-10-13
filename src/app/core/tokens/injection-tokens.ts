import { InjectionToken } from '@angular/core';
import { ApplicationsApi, AnimalsApi, AuthApi, FavoritesApi } from './api.tokens';

/**
 * Точки замены mock → реальный API.
 * Достаточно переопределить провайдер в app.config.ts:
 *   { provide: AUTH_API, useClass: HttpAuthApi }
 */
export const AUTH_API = new InjectionToken<AuthApi>('AUTH_API');
export const ANIMALS_API = new InjectionToken<AnimalsApi>('ANIMALS_API');
export const FAVORITES_API = new InjectionToken<FavoritesApi>('FAVORITES_API');
export const APPLICATIONS_API = new InjectionToken<ApplicationsApi>('APPLICATIONS_API');
