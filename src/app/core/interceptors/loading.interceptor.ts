import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';

import { LoaderService } from '../services/loader.service';

/** Показывает глобальный индикатор, пока выполняются запросы к API. */
export const loadingInterceptor: HttpInterceptorFn = (request, next) => {
  if (!request.url.startsWith('/api/')) {
    return next(request);
  }

  const loader = inject(LoaderService);
  loader.start();

  return next(request).pipe(finalize(() => loader.stop()));
};
