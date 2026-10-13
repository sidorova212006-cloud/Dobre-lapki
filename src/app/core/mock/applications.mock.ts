import { Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ApplicationsApi } from '../tokens/api.tokens';
import { Application, CreateApplicationDto } from '../models/application.model';
import { MOCK_ANIMALS } from './animals.data';
import { mockRequest } from './mock-utils';

const STORAGE_KEY = 'dp.applications';

function seed(): Application[] {
  const [first, fifth] = [MOCK_ANIMALS[0], MOCK_ANIMALS[4]];
  const items: Application[] = [];
  if (first) {
    items.push({ id: 1, animal: first, status: 'approved', createdAt: '2026-09-30T10:12:00.000Z' });
  }
  if (fifth) {
    items.push({ id: 2, animal: fifth, status: 'pending', createdAt: '2026-10-08T16:40:00.000Z' });
  }
  return items;
}

// TODO(BACKEND): заменить mock на реальный API.
@Injectable()
export class MockApplicationsApi implements ApplicationsApi {
  private readonly items = signal<Application[]>(this.read());
  private nextId = Math.max(0, ...this.read().map((item) => item.id)) + 1;

  create(dto: CreateApplicationDto): Observable<Application> {
    return mockRequest(
      () => {
        const animal = MOCK_ANIMALS.find((item) => item.id === dto.animalId);
        if (!animal) throw new Error('Животное не найдено');
        const application: Application = {
          id: this.nextId++,
          animal,
          status: 'pending',
          createdAt: new Date().toISOString(),
        };
        this.commit([...this.items(), application]);
        return application;
      },
      'Не удалось отправить заявку. Попробуйте ещё раз.',
    );
  }

  getMy(): Observable<Application[]> {
    return mockRequest(
      () => [...this.items()].sort((a, b) => b.createdAt.localeCompare(a.createdAt)),
      'Не удалось загрузить ваши заявки.',
    ).pipe(tap((items) => this.items.set(items)));
  }

  private commit(items: Application[]): void {
    this.items.set(items);
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem(
        STORAGE_KEY,
        JSON.stringify(items.map((item) => ({ ...item, animal: item.animal.id }))),
      );
    }
  }

  private read(): Application[] {
    if (typeof localStorage === 'undefined') return seed();
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return seed();
    try {
      const parsed: unknown = JSON.parse(raw);
      if (!Array.isArray(parsed)) return seed();
      return parsed
        .map((entry) => this.hydrate(entry))
        .filter((item): item is Application => item !== null);
    } catch {
      return seed();
    }
  }

  private hydrate(entry: unknown): Application | null {
    if (typeof entry !== 'object' || entry === null) return null;
    const record = entry as Record<string, unknown>;
    const animalId = record['animal'];
    const animal = MOCK_ANIMALS.find((item) => item.id === animalId);
    const status = record['status'];
    if (!animal || typeof record['id'] !== 'number' || typeof record['createdAt'] !== 'string') {
      return null;
    }
    if (status !== 'pending' && status !== 'approved' && status !== 'rejected') return null;
    return {
      id: record['id'],
      animal,
      status,
      createdAt: record['createdAt'],
    };
  }
}
