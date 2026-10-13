import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AUTH_API } from '../tokens/injection-tokens';
import { LoginDto, RegisterDto, User } from '../models/user.model';
import { ToastService } from './toast.service';

@Injectable({ providedIn: 'root' })
export class SessionService {
  private readonly api = inject(AUTH_API);
  private readonly toast = inject(ToastService);

  private readonly state = signal<User | null>(this.api.currentUser());

  readonly user = this.state.asReadonly();
  readonly isAuthenticated = computed(() => this.state() !== null);

  login(dto: LoginDto): Observable<User> {
    return this.api.login(dto).pipe(tap((user) => this.state.set(user)));
  }

  register(dto: RegisterDto): Observable<User> {
    return this.api.register(dto).pipe(tap((user) => this.state.set(user)));
  }

  logout(): void {
    this.api.logout().subscribe({
      next: () => {
        this.state.set(null);
        this.toast.info('Вы вышли из аккаунта');
      },
      error: () => this.toast.error('Не удалось выйти из аккаунта'),
    });
  }

  /** Локальное обновление профиля (mock: без бэкенда). */
  updateProfile(patch: Partial<Pick<User, 'name' | 'email' | 'phone'>>): void {
    const current = this.state();
    if (!current) return;
    const updated = { ...current, ...patch };
    this.state.set(updated);
    this.api.saveCurrentUser(updated);
  }
}
