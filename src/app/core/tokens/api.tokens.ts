import { Observable } from 'rxjs';
import { Animal } from '../models/animal.model';
import { AnimalFilters } from '../models/filters.model';
import { Application, CreateApplicationDto } from '../models/application.model';
import { LoginDto, RegisterDto, User } from '../models/user.model';

/**
 * Контракты API. Каждый контракт регистрируется через InjectionToken,
 * поэтому переход на реальный бэкенд = замена одного провайдера в app.config.ts.
 */

export interface AuthApi {
  login(dto: LoginDto): Observable<User>;
  register(dto: RegisterDto): Observable<User>;
  logout(): Observable<void>;
  /** Синхронно читает сохранённую сессию (localStorage). */
  currentUser(): User | null;
  /** Сохраняет изменения профиля текущего пользователя. */
  saveCurrentUser(user: User): void;
}

export interface AnimalsApi {
  getAnimals(filters?: Partial<AnimalFilters>): Observable<Animal[]>;
  getAnimalById(id: number): Observable<Animal>;
  getPopular(): Observable<Animal[]>;
}

export interface FavoritesApi {
  list(): Observable<number[]>;
  /** Возвращает обновлённый список id избранного. */
  toggle(animalId: number): Observable<number[]>;
  has(animalId: number): boolean;
}

export interface ApplicationsApi {
  create(dto: CreateApplicationDto): Observable<Application>;
  getMy(): Observable<Application[]>;
}
