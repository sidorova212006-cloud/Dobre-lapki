import { Injectable, inject, signal } from '@angular/core';
import { FAVORITES_API } from '../tokens/injection-tokens';
import { ToastService } from './toast.service';

@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly api = inject(FAVORITES_API);
  private readonly toast = inject(ToastService);

  private readonly ids = signal<number[]>([]);
  private readonly loading = signal(false);
  private loaded = false;

  /** Идемпотентная однократная загрузка избранного. */
  ensureLoaded(): void {
    if (this.loaded || this.loading()) return;
    this.loading.set(true);
    this.api.list().subscribe({
      next: (ids) => {
        this.ids.set(ids);
        this.loaded = true;
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
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
    this.api.toggle(animalId).subscribe({
      next: (ids) => {
        this.ids.set(ids);
        this.loaded = true;
        if (wasFavorite) {
          this.toast.info('Удалено из избранного');
        } else {
          this.toast.success('Добавлено в избранное');
        }
      },
      error: () => this.toast.error('Не удалось обновить избранное'),
    });
  }
}
