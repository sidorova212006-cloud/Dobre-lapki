import { Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { FavoritesApi } from '../tokens/api.tokens';
import { mockRequest } from './mock-utils';

const STORAGE_KEY = 'dp.favorites';

// TODO(BACKEND): заменить mock на реальный API (избранное пользователя).
@Injectable()
export class MockFavoritesApi implements FavoritesApi {
  private readonly ids = signal<number[]>(this.read());

  list(): Observable<number[]> {
    return mockRequest(() => [...this.ids()], 'Не удалось загрузить избранное.').pipe(
      tap((ids) => this.ids.set(ids)),
    );
  }

  toggle(animalId: number): Observable<number[]> {
    return mockRequest(
      () => {
        const current = this.ids();
        const next = current.includes(animalId)
          ? current.filter((id) => id !== animalId)
          : [...current, animalId];
        this.persist(next);
        return next;
      },
      'Не удалось обновить избранное.',
    ).pipe(tap((ids) => this.ids.set(ids)));
  }

  has(animalId: number): boolean {
    return this.ids().includes(animalId);
  }

  private persist(ids: number[]): void {
    this.ids.set(ids);
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(ids));
    }
  }

  private read(): number[] {
    if (typeof localStorage === 'undefined') return [];
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return [];
    try {
      const parsed: unknown = JSON.parse(raw);
      return Array.isArray(parsed) ? parsed.filter((id): id is number => typeof id === 'number') : [];
    } catch {
      return [];
    }
  }
}
