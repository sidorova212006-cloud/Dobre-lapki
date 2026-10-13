import { Observable, defer, of, throwError } from 'rxjs';
import { delay } from 'rxjs/operators';

/**
 * Общие утилиты mock-слоя: искусственная задержка и эмуляция ошибок.
 */

const MIN_DELAY = 300;
const MAX_DELAY = 800;

/**
 * Эмуляция ошибок. По умолчанию выключена, чтобы демо было стабильным.
 * Включить: пробросить ?mockError=1 в URL или выставить значение > 0.
 */
export const MOCK_ERROR_RATE = 0;

function shouldFail(): boolean {
  if (typeof location !== 'undefined') {
    const params = new URLSearchParams(location.search);
    if (params.has('mockError')) return true;
  }
  return MOCK_ERROR_RATE > 0 && Math.random() < MOCK_ERROR_RATE;
}

function randomDelay(): number {
  return MIN_DELAY + Math.random() * (MAX_DELAY - MIN_DELAY);
}

/**
 * Выполняет фабрику после задержки 300–800 мс.
 * При включённой эмуляции ошибок выбрасывает Error.
 */
export function mockRequest<T>(
  factory: () => T,
  errorMessage = 'Не удалось получить данные. Попробуйте ещё раз.',
): Observable<T> {
  return defer(() => {
    const latency = randomDelay();
    if (shouldFail()) {
      return throwError(() => new Error(errorMessage)).pipe(delay(latency));
    }
    try {
      return of(factory()).pipe(delay(latency));
    } catch (error) {
      return throwError(() => error as Error).pipe(delay(latency));
    }
  });
}
