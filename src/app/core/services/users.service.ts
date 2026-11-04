import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { LoginDto, RegisterDto, UpdateUserDto, User } from '../models/user.model';
import { ApiService } from './api.service';

/** Запросы пользователей: вход, регистрация и профиль. */
@Injectable({ providedIn: 'root' })
export class UsersService {
  private readonly api = inject(ApiService);

  login(dto: LoginDto): Observable<User> {
    return this.api.post<User>('/users/login', {
      email: dto.email,
      password: dto.password,
    });
  }

  register(dto: RegisterDto): Observable<User> {
    return this.api.post<User>('/users/register', {
      name: dto.name,
      email: dto.email,
      phone: dto.phone ?? null,
      password: dto.password,
    });
  }

  getById(id: number): Observable<User> {
    return this.api.get<User>(`/users/${id}`);
  }

  update(id: number, dto: UpdateUserDto): Observable<User> {
    return this.api.put<User>(`/users/${id}`, dto);
  }
}
