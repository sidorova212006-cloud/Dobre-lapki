import { Injectable, computed, signal } from '@angular/core';

/** Счётчик активных HTTP-запросов для глобального индикатора загрузки. */
@Injectable({ providedIn: 'root' })
export class LoaderService {
  private readonly pending = signal(0);
  readonly isLoading = computed(() => this.pending() > 0);

  start(): void {
    this.pending.update((value) => value + 1);
  }

  stop(): void {
    this.pending.update((value) => Math.max(0, value - 1));
  }
}
