import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import {
  Application,
  CreateApplicationDto,
} from '../models/application.model';
import { PagedResult } from '../models/paged-result';
import { ApiService } from './api.service';

/** Запросы заявок на усыновление. */
@Injectable({ providedIn: 'root' })
export class ApplicationsService {
  private readonly api = inject(ApiService);

  getMy(page = 1, pageSize = 100): Observable<PagedResult<Application>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.api.get<PagedResult<Application>>('/applications', params);
  }

  getById(id: number): Observable<Application> {
    return this.api.get<Application>(`/applications/${id}`);
  }

  create(dto: CreateApplicationDto): Observable<Application> {
    return this.api.post<Application>('/applications', dto);
  }
}
