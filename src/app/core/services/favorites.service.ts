import { HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';

import { PagedResult } from '../models/paged-result';
import { errorMessage } from '../utils/async-state';
import { ApiService } from './api.service';
import { ToastService } from './toast.service';

/** Избранное пользователя: список id и добавление/удаление через API. */
@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly api = inject(ApiService);
  private readonly toast = inject(ToastService);

  private readonly ids = signal<number[]>([]);
  private readonly loading = signal(false);
  private loaded = false;

  /** Идемпотентная однократная загрузка избранного. */
  ensureLoaded(): void {
    if (this.loaded || this.loading()) return;
    this.load();
  }

  /** Принудительное обновление (например, кнопка «Повторить»). */
  reload(): void {
    this.load();
  }

  isFavorite(animalId: number): boolean {
    return this.ids().includes(animalId);
  }

  count(): number {
    return this.ids().length;
  }

  list(): number[] {
    return this.ids();
  }

  toggle(animalId: number): void {
    const wasFavorite = this.isFavorite(animalId);
    const request = wasFavorite
      ? this.api.delete<void>(`/favorites/${animalId}`)
      : this.api.post<void>(`/favorites/${animalId}`);

    request.subscribe({
      next: () => {
        this.ids.update((ids) =>
          wasFavorite ? ids.filter((id) => id !== animalId) : [...ids, animalId],
        );
        this.loaded = true;
        if (wasFavorite) {
          this.toast.info('Удалено из избранного');
        } else {
          this.toast.success('Добавлено в избранное');
        }
      },
      error: (error: unknown) => this.toast.error(errorMessage(error)),
    });
  }

  private load(): void {
    this.loading.set(true);
    const params = new HttpParams().set('page', 1).set('pageSize', 100);

    this.api.get<PagedResult<number>>('/favorites', params).subscribe({
      next: (page) => {
        this.ids.set(page.items);
        this.loaded = true;
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
