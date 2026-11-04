import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap, throwError } from 'rxjs';

import { LoginDto, RegisterDto, UpdateUserDto, User } from '../models/user.model';
import { ToastService } from './toast.service';
import { UsersService } from './users.service';

const STORAGE_KEY = 'dp.session';

/** Сессия пользователя: вход/регистрация через API и хранение профиля локально. */
@Injectable({ providedIn: 'root' })
export class SessionService {
  private readonly users = inject(UsersService);
  private readonly toast = inject(ToastService);

  private readonly state = signal<User | null>(this.read());

  readonly user = this.state.asReadonly();
  readonly isAuthenticated = computed(() => this.state() !== null);

  login(dto: LoginDto): Observable<User> {
    return this.users
      .login(dto)
      .pipe(tap((user) => this.persist(user, dto.remember ?? false)));
  }

  register(dto: RegisterDto): Observable<User> {
    return this.users.register(dto).pipe(tap((user) => this.persist(user, true)));
  }

  logout(): void {
    this.state.set(null);
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem(STORAGE_KEY);
      sessionStorage.removeItem(STORAGE_KEY);
    }
    this.toast.info('Вы вышли из аккаунта');
  }

  updateProfile(dto: UpdateUserDto): Observable<User> {
    const current = this.state();
    if (!current) {
      return throwError(() => new Error('Пользователь не авторизован'));
    }

    const remember =
      typeof localStorage !== 'undefined' && localStorage.getItem(STORAGE_KEY) !== null;

    return this.users
      .update(current.id, dto)
      .pipe(tap((user) => this.persist(user, remember)));
  }

  private persist(user: User, remember: boolean): void {
    this.state.set(user);
    if (typeof localStorage === 'undefined') return;

    const target = remember ? localStorage : sessionStorage;
    const other = remember ? sessionStorage : localStorage;
    target.setItem(STORAGE_KEY, JSON.stringify(user));
    other.removeItem(STORAGE_KEY);
  }

  private read(): User | null {
    if (typeof localStorage === 'undefined') return null;

    const raw = localStorage.getItem(STORAGE_KEY) ?? sessionStorage.getItem(STORAGE_KEY);
    if (!raw) return null;

    try {
      const parsed = JSON.parse(raw) as Partial<User> | null;
      if (!parsed || typeof parsed.id !== 'number' || typeof parsed.email !== 'string') {
        return null;
      }

      return {
        id: parsed.id,
        name: parsed.name ?? '',
        email: parsed.email,
        phone: parsed.phone ?? '',
        avatar: parsed.avatar ?? '',
      };
    } catch {
      return null;
    }
  }
}
