import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { AnimalsApi } from '../tokens/api.tokens';
import { Animal } from '../models/animal.model';
import { AnimalFilters, applyFilters } from '../models/filters.model';
import { MOCK_ANIMALS } from './animals.data';
import { mockRequest } from './mock-utils';

// TODO(BACKEND): заменить mock на реальный API.
@Injectable()
export class MockAnimalsApi implements AnimalsApi {
  getAnimals(filters?: Partial<AnimalFilters>): Observable<Animal[]> {
    return mockRequest(
      () => applyFilters(MOCK_ANIMALS, filters ?? {}),
      'Не удалось загрузить список животных.',
    );
  }

  getAnimalById(id: number): Observable<Animal> {
    return mockRequest(
      () => {
        const animal = MOCK_ANIMALS.find((item) => item.id === id);
        if (!animal) throw new Error(`Животное с id=${id} не найдено`);
        return animal;
      },
      'Не удалось загрузить информацию о животном.',
    );
  }

  getPopular(): Observable<Animal[]> {
    // «Популярные» — детерминированная выборка: самые «свежие» 5 карточек.
    return mockRequest(() => applyFilters(MOCK_ANIMALS, { sort: 'newest' }).slice(0, 5)).pipe(
      map((animals) => animals),
    );
  }
}
