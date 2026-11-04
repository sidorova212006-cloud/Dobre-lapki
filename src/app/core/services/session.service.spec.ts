import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { User } from '../models/user.model';
import { SessionService } from './session.service';

const USER: User = {
  id: 1,
  name: 'Анна',
  email: 'user@example.com',
  phone: '+7 900 000-00-00',
  avatar: 'https://example.com/avatar.png',
};

describe('SessionService', () => {
  let service: SessionService;
  let http: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();

    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(SessionService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
    localStorage.clear();
    sessionStorage.clear();
  });

  it('входит через API и сохраняет сессию', () => {
    service
      .login({ email: 'user@example.com', password: 'password123', remember: true })
      .subscribe();

    const request = http.expectOne('/api/v1/users/login');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      email: 'user@example.com',
      password: 'password123',
    });
    request.flush(USER);

    expect(service.user()).toEqual(USER);
    expect(service.isAuthenticated()).toBeTrue();
    expect(JSON.parse(localStorage.getItem('dp.session')!)).toEqual(USER);
  });

  it('регистрирует пользователя и сохраняет его', () => {
    service
      .register({ name: 'Анна', email: 'anna@example.com', password: 'secret123' })
      .subscribe();

    const request = http.expectOne('/api/v1/users/register');
    expect(request.request.body).toEqual({
      name: 'Анна',
      email: 'anna@example.com',
      phone: null,
      password: 'secret123',
    });
    request.flush({ ...USER, email: 'anna@example.com' });

    expect(service.user()?.email).toBe('anna@example.com');
  });

  it('обновляет профиль через API', () => {
    service
      .login({ email: 'user@example.com', password: 'password123', remember: true })
      .subscribe();
    http.expectOne('/api/v1/users/login').flush(USER);

    service
      .updateProfile({
        name: 'Анна Смирнова',
        email: 'user@example.com',
        phone: '+7 900 111-22-33',
      })
      .subscribe();

    const request = http.expectOne('/api/v1/users/1');
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual({
      name: 'Анна Смирнова',
      email: 'user@example.com',
      phone: '+7 900 111-22-33',
    });
    request.flush({ ...USER, name: 'Анна Смирнова' });

    expect(service.user()?.name).toBe('Анна Смирнова');
  });

  it('выходит и очищает сессию', () => {
    service
      .login({ email: 'user@example.com', password: 'password123', remember: true })
      .subscribe();
    http.expectOne('/api/v1/users/login').flush(USER);

    service.logout();

    expect(service.user()).toBeNull();
    expect(service.isAuthenticated()).toBeFalse();
    expect(localStorage.getItem('dp.session')).toBeNull();
  });
});
