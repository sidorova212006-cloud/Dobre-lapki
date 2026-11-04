import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { FavoritesService } from './favorites.service';

describe('FavoritesService', () => {
  let service: FavoritesService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(FavoritesService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('загружает список id избранного', () => {
    service.ensureLoaded();

    const request = http.expectOne((req) => req.url === '/api/v1/favorites');
    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('pageSize')).toBe('100');
    request.flush({ items: [1, 2], page: 1, pageSize: 100, totalCount: 2 });

    expect(service.list()).toEqual([1, 2]);
    expect(service.count()).toBe(2);
    expect(service.isFavorite(1)).toBeTrue();
  });

  it('добавляет животное в избранное', () => {
    service.ensureLoaded();
    http
      .expectOne((req) => req.url === '/api/v1/favorites')
      .flush({ items: [], page: 1, pageSize: 100, totalCount: 0 });

    service.toggle(5);

    const request = http.expectOne('/api/v1/favorites/5');
    expect(request.request.method).toBe('POST');
    request.flush(null, { status: 201, statusText: 'Created' });

    expect(service.isFavorite(5)).toBeTrue();
  });

  it('удаляет животное из избранного', () => {
    service.ensureLoaded();
    http
      .expectOne((req) => req.url === '/api/v1/favorites')
      .flush({ items: [5], page: 1, pageSize: 100, totalCount: 1 });

    service.toggle(5);

    const request = http.expectOne('/api/v1/favorites/5');
    expect(request.request.method).toBe('DELETE');
    request.flush(null, { status: 204, statusText: 'No Content' });

    expect(service.isFavorite(5)).toBeFalse();
    expect(service.count()).toBe(0);
  });
});
