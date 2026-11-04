import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';

import { ApiError, ErrorResponse } from '../models/api-error';

/**
 * Тонкая обёртка над HttpClient: единый базовый URL, JSON и обработка ошибок.
 * Все ошибки логируются с деталями и преобразуются в ApiError.
 */
@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/v1';

  get<T>(path: string, params?: HttpParams): Observable<T> {
    return this.http
      .get<T>(this.url(path), { params })
      .pipe(catchError((error: HttpErrorResponse) => this.handleError(error)));
  }

  post<T>(path: string, body?: unknown): Observable<T> {
    return this.http
      .post<T>(this.url(path), body)
      .pipe(catchError((error: HttpErrorResponse) => this.handleError(error)));
  }

  put<T>(path: string, body: unknown): Observable<T> {
    return this.http
      .put<T>(this.url(path), body)
      .pipe(catchError((error: HttpErrorResponse) => this.handleError(error)));
  }

  delete<T = void>(path: string): Observable<T> {
    return this.http
      .delete<T>(this.url(path))
      .pipe(catchError((error: HttpErrorResponse) => this.handleError(error)));
  }

  private url(path: string): string {
    return `${this.baseUrl}${path}`;
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    const payload = error.error as ErrorResponse | string | null;
    const serverMessage =
      typeof payload === 'string' ? payload : (payload?.message ?? '');
    const message =
      error.status === 0
        ? 'Не удалось связаться с сервером. Проверьте, что API запущен.'
        : serverMessage || `Ошибка запроса (${error.status}). Попробуйте ещё раз.`;

    // Детали ответа сохраняем в консоли для отладки.
    console.error(`[API] ${error.status} ${error.url ?? ''}`, {
      message,
      payload: error.error,
    });

    return throwError(() => new ApiError(message, error.status, error.url ?? ''));
  }
}
