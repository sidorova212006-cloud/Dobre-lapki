# Добрые лапки — приют для животных

Full-stack проект сайта приюта «Добрые лапки»:

- **Frontend** — SPA на **Angular 20** (standalone-компоненты, сигналы, Reactive Forms).
- **Backend** — **ASP.NET Core Web API (.NET 8)** с трёхслойной архитектурой, Swagger и xUnit-тестами.

Данные хранятся в памяти (без БД и ORM): репозитории спрятаны за интерфейсами, поэтому замена
хранилища на реальную БД не затронет домен и контроллеры. Frontend работает с API по HTTP,
mock-слоя больше нет.

---

## Запуск

Требования: **Node.js 20+** и **.NET 8 SDK**.

### 1. Backend

```bash
dotnet run --project backend/DobreLapki.Presentation
```

API: `http://localhost:5254`, Swagger (в Development): `http://localhost:5254/swagger`.

### 2. Frontend

```bash
npm install
npm start          # ng serve → http://localhost:4200
```

Dev-сервер проксирует `/api` на `http://localhost:5254` (`proxy.conf.json`), поэтому запускать
нужно **оба** процесса. В production SPA и API должны быть на одном origin (или настроен
обратный прокси).

### Сборка и тесты

```bash
npm run build                                      # production-сборка SPA
npm test -- --watch=false --browsers=ChromeHeadless # unit-тесты (Karma/Jasmine)
dotnet test backend/DobreLapki.sln                 # unit + integration тесты
```

### Демо-аккаунт

| Поле   | Значение           |
| ------ | ------------------ |
| Email  | `user@example.com` |
| Пароль | `password123`      |

Токенов и JWT нет: `POST /api/v1/users/login` возвращает профиль пользователя, а SPA хранит его
в `localStorage` (или `sessionStorage`, если снять галочку «Запомнить меня»). `/profile` и
`/apply/:animalId` защищены `authGuard`.

> In-memory хранилище очищается при перезапуске API: 15 животных, 2 заявки и демо-пользователь
> создаются заново из `SeedData`.

---

## Архитектура backend

```
backend/
├── DobreLapki.sln
├── DobreLapki.Domain/          # сущности, правила, сервисы, интерфейсы репозиториев — без зависимостей
├── DobreLapki.Infrastructure/  # in-memory репозитории, PBKDF2, MockFileStorage, seed-данные
├── DobreLapki.Presentation/    # контроллеры, DTO, middleware, Program.cs, конфигурация
└── DobreLapki.Tests/           # xUnit: домен, репозитории, интеграционные тесты API
```

- **Domain** не ссылается на Infrastructure/Presentation. Здесь живут `Animal`, `Application`,
  `User`, доменные правила (`AnimalRules`, `ApplicationRules`, `UserRules`), `TextSanitizer`
  (защита от XSS), `PaginationGuard`, `IFileStorage`, `IPasswordHasher` и доменные сервисы.
- **Infrastructure** реализует репозитории поверх `List<T>`/`Dictionary<TKey,TValue>` с `lock`,
  PBKDF2-хеширование паролей и заглушку файлового хранилища.
- **Presentation** — тонкие контроллеры: валидация DTO, маппинг в доменные черновики, вызов
  сервисов. Ошибки обрабатываются глобально.
- Настройки — через `IOptions<T>` с `ValidateDataAnnotations()` и `ValidateOnStart()`.
  Секреты в git не попадают: шаблон `appsettings.Secrets.example.json` копируется в
  `appsettings.Secrets.json` (файл в `.gitignore`).

### API

Базовый путь: `/api/v1`. Все списки постраничные: `{ items, page, pageSize, totalCount }`.

| Метод | Путь | Назначение |
| ----- | ---- | ---------- |
| `GET` | `/health` | Проверка доступности |
| `GET` | `/animals` | Каталог: `page`, `pageSize`, `types`, `genders`, `ages`, `search`, `sort` |
| `GET` | `/animals/popular?count=5` | Популярные животные |
| `GET` | `/animals/{id}` | Карточка животного |
| `POST` | `/animals` | Создать животное (`201` + `Location`) |
| `PUT` | `/animals/{id}` | Обновить животное |
| `DELETE` | `/animals/{id}` | Удалить животное (`204`) |
| `GET` | `/applications` | Заявки (постранично, сначала новые) |
| `GET` | `/applications/{id}` | Заявка по id |
| `POST` | `/applications` | Создать заявку (`201` + `Location`) |
| `PUT` | `/applications/{id}` | Изменить статус заявки |
| `DELETE` | `/applications/{id}` | Удалить заявку (`204`) |
| `GET` | `/users` | Пользователи (постранично) |
| `GET` | `/users/{id}` | Пользователь по id |
| `POST` | `/users/register` | Регистрация (`201` + `Location`) |
| `POST` | `/users/login` | Вход (`200`, без токена) |
| `PUT` | `/users/{id}` | Обновить профиль |
| `DELETE` | `/users/{id}` | Удалить пользователя (`204`) |
| `GET` | `/favorites` | Постраничный список id избранного |
| `POST` | `/favorites/{animalId}` | Добавить в избранное (`201`) |
| `DELETE` | `/favorites/{animalId}` | Удалить из избранного (`204`) |

Фильтры-мультивыборы передаются через запятую: `types=cat,dog`, `genders=male`,
`ages=under1,from1to3,over3`. Сортировка: `newest`, `oldest`, `name`. `pageSize` ограничен
доменом (максимум 100).

Коды ответов: `200`, `201` + `Location`, `204`, `400`, `404`, `409`, `500`. Любая ошибка имеет
единый формат:

```json
{
  "code": "validation_error",
  "message": "Кличка обязательна.",
  "traceId": "0HNPQL727K74P:00000006"
}
```

Валидация выполняется на трёх уровнях: атрибуты DTO, доменные правила и проверки сервисов.
Строки проходят санитизацию (`TextSanitizer`), все асинхронные методы принимают
`CancellationToken`, `.Result`/`.Wait()` не используются.

---

## Frontend: работа с API

- `core/services/api.service.ts` — тонкая обёртка над `HttpClient`: базовый URL `/api/v1`,
  `get/post/put/delete`, `catchError` с логированием деталей в консоль и преобразованием в
  `ApiError` (`code`, `message`, `traceId`).
- `core/services/animals.service.ts`, `applications.service.ts`, `users.service.ts` — фичевые
  сервисы; `session.service.ts` и `favorites.service.ts` используют их.
- `core/interceptors/loading.interceptor.ts` + `LoaderService` — глобальный тонкий индикатор
  загрузки вверху страницы. Локальные спиннеры — у кнопок отправки форм (`ButtonComponent`
  с `[loading]`).
- Модели соответствуют DTO backend: `PagedResult<T>`, `Animal`, `Application` (включая
  `fullName`, `email`, `phone`, `about`), `User`, `LoginDto`, `RegisterDto`, `UpdateUserDto`.
- Все ошибки списков показываются через `ErrorStateComponent` с кнопкой «Повторить», ошибки
  форм — toast + сообщение под полем/формой.

---

## Структура

```
src/
├── index.html                     # Manrope (Google Fonts), lang="ru"
├── styles.scss                    # точка входа глобальных стилей
├── styles/
│   ├── _tokens.scss               # дизайн-токены (CSS-переменные)
│   ├── _controls.scss             # кнопки, поля, бейджи, сетки, скелетоны
│   └── _auth.scss                 # общие стили форм входа/регистрации
└── app/
    ├── app.config.ts              # роутер + provideHttpClient + interceptor
    ├── app.routes.ts              # ленивые маршруты (loadComponent)
    ├── app.ts / app.html / app.scss
    ├── core/
    │   ├── models/                # Animal, Application, User, PagedResult, ApiError, фильтры
    │   ├── interceptors/          # loading.interceptor.ts
    │   ├── services/              # ApiService, AnimalsService, ApplicationsService,
    │   │                          # UsersService, SessionService, FavoritesService, LoaderService
    │   ├── guards/auth.guard.ts   # AuthGuard для /profile и /apply
    │   └── utils/                 # async-state, form-errors
    ├── layout/
    │   ├── header/                # sticky-шапка, бургер, поиск, избранное, аватар
    │   ├── footer/                # подвал
    │   └── auth-layout/           # двухпанельный экран без шапки
    ├── shared/ui/                 # мини-библиотека UI-компонентов
    │   ├── button, badge, animal-card, input, textarea, checkbox, select,
    │   ├── empty-state, skeleton, heart-button, error-state, icon, toast
    └── pages/                     # экраны (lazy)
        ├── home, animal-detail, about, contacts
        ├── catalog, favorites, profile, apply
        ├── login, register
        └── not-found
```

### Маршруты

| Путь               | Экран                                             |
| ------------------ | ------------------------------------------------- |
| `/`                | Главная: hero-карусель (3 слайда) + популярные    |
| `/animals`         | Каталог с фильтрами и сортировкой                 |
| `/animals/:id`     | Карточка животного с галереей                     |
| `/login`           | Вход (без шапки)                                  |
| `/register`        | Регистрация (без шапки)                           |
| `/apply/:animalId` | Заявка на усыновление (только для авторизованных) |
| `/profile`         | Профиль и «Мои заявки» (только для авторизованных)|
| `/favorites`       | Избранное                                         |
| `/about`           | О нас                                             |
| `/contacts`        | Контакты                                          |
| `**`               | 404 «Страница не найдена»                         |

---

## Тесты

- **Backend:** 71 тест — доменные правила и `PaginationGuard`, in-memory репозитории,
  интеграционные тесты через `WebApplicationFactory` (статусы, `Location`, единый формат ошибок,
  пагинация, избранное, вход/регистрация).
- **Frontend:** Karma/Jasmine + `HttpTestingController` — `ApiService` (включая преобразование
  ошибок), `AnimalsService` (параметры фильтров и пагинации), `SessionService` (вход,
  регистрация, профиль), `FavoritesService`, а также smoke-тест `App`.

---

## Изображения

Фото животных приходят из API и в seed-данных заданы ссылками на `picsum.photos`. Если внешний
хост недоступен (офлайн, прокси, блокировка), все `<img>` используют директиву `appImgFallback`
(`shared/ui/img-fallback/img-fallback.directive.ts`): при ошибке загрузки подставляется локальная
SVG-заглушка (data URI, без сети).

---

## Дизайн-токены

Определены как CSS-переменные в `src/styles/_tokens.scss`.

| Переменная | Значение | Назначение |
| ---------- | -------- | ---------- |
| `--color-bg` | `#F6F4EF` | кремовый фон страниц |
| `--color-surface` | `#FFFFFF` | карточки, шапка |
| `--color-primary` | `#2F5D50` | кнопки, активные элементы |
| `--color-primary-hover` | `#265044` | hover основного цвета |
| `--color-primary-soft` | `#E4EDE9` | светлые подложки, бейджи |
| `--color-text` | `#1E2823` | основной текст |
| `--color-text-muted` | `#7C857F` | вторичный текст, метаданные |
| `--color-border` | `#E8E4DB` | бордеры карточек и инпутов |
| `--color-badge-pending-bg/text` | `#F7EDDD` / `#96762E` | бейдж «На рассмотрении» |
| `--color-badge-approved-bg/text` | `#E3EFE7` / `#2F5D50` | бейдж «Одобрено» |
| `--color-badge-rejected-bg/text` | `#F7E3DE` / `#A6503C` | бейдж «Отказано» |

Дополнительно:

- Радиусы: `--radius-sm: 10px` (кнопки/инпуты), `--radius-md: 16px` (карточки),
  `--radius-lg: 24px` (hero/баннеры), `--radius-pill: 999px` (бейджи, аватары).
- Тени: `--shadow-card-hover` — единственная, только на hover карточек.
- Сетка: `--container: 1200px`, центрирование через `.container`.
- Отступы: `--space-1…--space-16` кратны 4px.
- Типографика: Manrope + системный fallback. H1 32–40px/800, H2 22–24px/700, body 14–16px,
  метаданные 13–14px приглушённым цветом.

---

## Принятые решения

- **Иконки.** Официальные иконки `lucide-angular` с локальным резолвом вместо DI-токена
  `LUCIDE_ICONS`: набор `APP_ICONS` импортирует нужные иконки, а `IconComponent` выбирает иконку
  по kebab-имени и рендерит через `<lucide-icon [img]>`.
- **Без JWT.** В учебном backend авторизация сведена к простым endpoint'ам входа/регистрации:
  сервер возвращает профиль, SPA хранит его локально. Токены и защита маршрутов на сервере не
  добавлялись.
- **Избранное** — отдельные add/remove endpoint'ы (`POST`/`DELETE /favorites/{animalId}`),
  список отдаётся постранично как id, чтобы не дублировать данные животных.
- **Пароли** хешируются PBKDF2 (`PasswordHasher`), в открытом виде не хранятся и не логируются.
- **Секреты** — `appsettings.Secrets.example.json` как шаблон; реальный
  `appsettings.Secrets.json` исключён из git.
- **Ошибки** — единый `ErrorResponse` для middleware, `InvalidModelStateResponseFactory` и
  доменных исключений (400/404/409), поэтому клиент всегда получает `{code,message,traceId}`.
- **Поиск в шапке** ведёт в каталог с `?q=`; каталог передаёт это как `search` в API.
- **«Забыли пароль?»** — toast-заглушка: восстановления пароля в учебном API нет.
- **Фото** — seed-ссылки `picsum.photos`; директиву `appImgFallback` решено оставить как защиту
  от битых внешних ссылок.
- **Сортировка каталога** вынесена в отдельный `FormControl`, потому что она находится вне
  блока фильтров и не должна сбрасываться вместе с ними.

---

## Проверка качества

- `npm run build` проходит без ошибок (production); `strict` + `strictTemplates`, без `any`.
- `dotnet build` — без предупреждений и ошибок; `dotnet test` — 71/71.
- `npm test` — 14/14 в headless Chrome.
- Все списки: skeleton → ошибка с кнопкой «Повторить» → пустое состояние.
- Формы: валидация, блокировка кнопки при невалидности/отправке, локальный спиннер, ошибки под
  полями, серверные ошибки — toast + консоль.
- Адаптив: сетка каталога 3 → 2 → 1, мобильная панель фильтров, бургер-меню в шапке.
- Доступность: семантические теги, `label`, `aria-label`/`aria-pressed`/`aria-expanded` на
  иконочных кнопках, `role="progressbar"` у глобального индикатора, видимые `:focus-visible`.
