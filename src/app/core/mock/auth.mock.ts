import { Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AuthApi } from '../tokens/api.tokens';
import { LoginDto, RegisterDto, User } from '../models/user.model';
import { mockRequest } from './mock-utils';

const STORAGE_KEY = 'dp.session';
const AVATAR_SEED = 'dp-user';

// TODO(BACKEND): заменить mock на реальный API (HTTP + токены).
@Injectable()
export class MockAuthApi implements AuthApi {
  private readonly session = signal<User | null>(this.read());

  login(dto: LoginDto): Observable<User> {
    const name = dto.email.split('@')[0] || 'Пользователь';
    return mockRequest(
      () => this.buildUser(dto.email, name),
      'Не удалось войти. Проверьте email и пароль.',
    ).pipe(tap((user) => this.persist(user, dto.remember ?? false)));
  }

  register(dto: RegisterDto): Observable<User> {
    return mockRequest(
      () => this.buildUser(dto.email, dto.name, dto.phone),
      'Не удалось завершить регистрацию. Попробуйте позже.',
    ).pipe(tap((user) => this.persist(user, true)));
  }

  logout(): Observable<void> {
    return mockRequest(() => {
      this.session.set(null);
      if (typeof localStorage !== 'undefined') {
        localStorage.removeItem(STORAGE_KEY);
        sessionStorage.removeItem(STORAGE_KEY);
      }
      return undefined;
    });
  }

  saveCurrentUser(user: User): void {
    this.session.set(user);
    if (typeof localStorage === 'undefined') return;
    // Обновляем ту же запись, в которой уже лежит сессия.
    if (localStorage.getItem(STORAGE_KEY)) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
    } else if (sessionStorage.getItem(STORAGE_KEY)) {
      sessionStorage.setItem(STORAGE_KEY, JSON.stringify(user));
    }
  }

  currentUser(): User | null {
    return this.session();
  }

  private buildUser(email: string, name: string, phone = '+7 900 000-00-00'): User {
    return {
      id: 1,
      name: name || 'Пользователь',
      email,
      phone,
      avatar: `https://picsum.photos/seed/${AVATAR_SEED}/200/200`,
    };
  }

  private persist(user: User, remember: boolean): void {
    this.session.set(user);
    if (typeof localStorage === 'undefined') return;
    const target = remember ? localStorage : sessionStorage;
    target.setItem(STORAGE_KEY, JSON.stringify(user));
  }

  private read(): User | null {
    if (typeof localStorage === 'undefined') return null;
    const raw = localStorage.getItem(STORAGE_KEY) ?? sessionStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as User;
    } catch {
      return null;
    }
  }
}
