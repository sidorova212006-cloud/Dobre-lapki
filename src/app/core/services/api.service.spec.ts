import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { ApiError } from '../models/api-error';
import { ApiService } from './api.service';

describe('ApiService', () => {
  let service: ApiService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(ApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('выполняет GET и возвращает данные', () => {
    let value: unknown;

    service.get<{ status: string }>('/health').subscribe((data) => (value = data));

    const request = http.expectOne('/api/v1/health');
    expect(request.request.method).toBe('GET');
    request.flush({ status: 'ok' });

    expect(value).toEqual({ status: 'ok' });
  });

  it('преобразует ошибку API в ApiError и пишет детали в консоль', () => {
    const consoleSpy = spyOn(console, 'error');
    let received: unknown;

    service.get('/animals/999').subscribe({
      error: (error: unknown) => (received = error),
    });

    const request = http.expectOne('/api/v1/animals/999');
    request.flush(
      { code: 'not_found', message: 'Животное не найдено', traceId: 'trace-1' },
      { status: 404, statusText: 'Not Found' },
    );

    expect(received).toBeInstanceOf(ApiError);
    expect((received as ApiError).status).toBe(404);
    expect((received as ApiError).message).toBe('Животное не найдено');
    expect(consoleSpy).toHaveBeenCalled();
  });
});
