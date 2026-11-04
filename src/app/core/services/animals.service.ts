import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Animal } from '../models/animal.model';
import { AnimalFilters } from '../models/filters.model';
import { PagedResult } from '../models/paged-result';
import { ApiService } from './api.service';

/** Запросы каталога животных. */
@Injectable({ providedIn: 'root' })
export class AnimalsService {
  private readonly api = inject(ApiService);

  getAnimals(
    filters: Partial<AnimalFilters> = {},
    page = 1,
    pageSize = 100,
  ): Observable<PagedResult<Animal>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);

    if (filters.types?.length) params = params.set('types', filters.types.join(','));
    if (filters.genders?.length) params = params.set('genders', filters.genders.join(','));
    if (filters.ages?.length) params = params.set('ages', filters.ages.join(','));

    const search = filters.search?.trim();
    if (search) params = params.set('search', search);
    if (filters.sort) params = params.set('sort', filters.sort);

    return this.api.get<PagedResult<Animal>>('/animals', params);
  }

  getAnimalById(id: number): Observable<Animal> {
    return this.api.get<Animal>(`/animals/${id}`);
  }

  getPopular(count = 5): Observable<Animal[]> {
    return this.api.get<Animal[]>(
      '/animals/popular',
      new HttpParams().set('count', count),
    );
  }
}
