import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { AnimalsService } from './animals.service';

describe('AnimalsService', () => {
  let service: AnimalsService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(AnimalsService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('передаёт фильтры, сортировку и пагинацию', () => {
    let total = -1;

    service
      .getAnimals(
        {
          types: ['cat', 'dog'],
          genders: ['female'],
          ages: ['under1'],
          search: ' мур ',
          sort: 'name',
        },
        2,
        5,
      )
      .subscribe((page) => (total = page.totalCount));

    const request = http.expectOne((req) => req.url === '/api/v1/animals');
    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('page')).toBe('2');
    expect(request.request.params.get('pageSize')).toBe('5');
    expect(request.request.params.get('types')).toBe('cat,dog');
    expect(request.request.params.get('genders')).toBe('female');
    expect(request.request.params.get('ages')).toBe('under1');
    expect(request.request.params.get('search')).toBe('мур');
    expect(request.request.params.get('sort')).toBe('name');

    request.flush({ items: [], page: 2, pageSize: 5, totalCount: 0 });
    expect(total).toBe(0);
  });

  it('не отправляет пустые фильтры', () => {
    service.getAnimals({ types: [], genders: [], ages: [], sort: 'newest' }).subscribe();

    const request = http.expectOne((req) => req.url === '/api/v1/animals');
    expect(request.request.params.keys()).toEqual(['page', 'pageSize', 'sort']);
    request.flush({ items: [], page: 1, pageSize: 100, totalCount: 0 });
  });

  it('запрашивает популярных животных', () => {
    let count = -1;

    service.getPopular(3).subscribe((items) => (count = items.length));

    const request = http.expectOne((req) => req.url === '/api/v1/animals/popular');
    expect(request.request.params.get('count')).toBe('3');
    request.flush([]);

    expect(count).toBe(0);
  });

  it('запрашивает животное по идентификатору', () => {
    service.getAnimalById(7).subscribe();

    const request = http.expectOne('/api/v1/animals/7');
    expect(request.request.method).toBe('GET');
    request.flush({ id: 7, name: 'Барсик' });
  });
});
