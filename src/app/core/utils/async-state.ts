import { Signal, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { Observable, catchError, map, of, startWith, switchMap } from 'rxjs';

/** Единая модель состояния асинхронной загрузки для списков и деталей. */
export type AsyncState<T> =
  | { readonly status: 'loading' }
  | { readonly status: 'success'; readonly data: T }
  | { readonly status: 'error'; readonly error: string };

export interface AsyncStateHandle<T> {
  readonly state: Signal<AsyncState<T>>;
  /** Повторный запрос (в т.ч. кнопка «Повторить»). */
  reload(): void;
}

export function errorMessage(error: unknown): string {
  return error instanceof Error && error.message
    ? error.message
    : 'Произошла ошибка. Попробуйте ещё раз.';
}

/**
 * Оборачивает Observable в сигнал состояния (loading/success/error).
 * Реактивность через toObservable + toSignal.
 */
export function asyncState<T>(factory: () => Observable<T>): AsyncStateHandle<T> {
  const reload = signal(0);

  const state = toSignal(
    toObservable(reload).pipe(
      switchMap(() =>
        factory().pipe(
          map((data): AsyncState<T> => ({ status: 'success', data })),
          catchError((error: unknown) =>
            of<AsyncState<T>>({ status: 'error', error: errorMessage(error) }),
          ),
          startWith<AsyncState<T>>({ status: 'loading' }),
        ),
      ),
    ),
    { initialValue: { status: 'loading' } },
  );

  return { state, reload: () => reload.update((value) => value + 1) };
}
