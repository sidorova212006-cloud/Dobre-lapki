import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    title: 'Добрые лапки — приют для животных',
    loadComponent: () => import('./pages/home/home').then((m) => m.HomePage),
  },
  {
    path: 'animals',
    title: 'Все животные — Добрые лапки',
    loadComponent: () => import('./pages/catalog/catalog').then((m) => m.CatalogPage),
  },
  {
    path: 'animals/:id',
    title: 'Карточка животного — Добрые лапки',
    loadComponent: () => import('./pages/animal-detail/animal-detail').then((m) => m.AnimalDetailPage),
  },
  {
    path: 'login',
    title: 'Вход — Добрые лапки',
    loadComponent: () => import('./pages/login/login').then((m) => m.LoginPage),
  },
  {
    path: 'register',
    title: 'Регистрация — Добрые лапки',
    loadComponent: () => import('./pages/register/register').then((m) => m.RegisterPage),
  },
  {
    path: 'apply/:animalId',
    title: 'Заявка на усыновление — Добрые лапки',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/apply/apply').then((m) => m.ApplyPage),
  },
  {
    path: 'profile',
    title: 'Профиль — Добрые лапки',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/profile/profile').then((m) => m.ProfilePage),
  },
  {
    path: 'favorites',
    title: 'Избранное — Добрые лапки',
    loadComponent: () => import('./pages/favorites/favorites').then((m) => m.FavoritesPage),
  },
  {
    path: 'about',
    title: 'О нас — Добрые лапки',
    loadComponent: () => import('./pages/about/about').then((m) => m.AboutPage),
  },
  {
    path: 'contacts',
    title: 'Контакты — Добрые лапки',
    loadComponent: () => import('./pages/contacts/contacts').then((m) => m.ContactsPage),
  },
  {
    path: '**',
    title: 'Страница не найдена — Добрые лапки',
    loadComponent: () => import('./pages/not-found/not-found').then((m) => m.NotFoundPage),
  },
];
