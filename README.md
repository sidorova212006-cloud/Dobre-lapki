# Добрые лапки — приют для животных

Frontend сайта приюта для животных «Добрые лапки» на **Angular 20**. Бэкенда нет: все данные
приходят из **mock-сервисов** (in-memory, с задержкой 300–800 мс и эмуляцией ошибок). Проект
полностью готов к подключению реального API — достаточно заменить провайдеры в `app.config.ts`.

---

## Запуск

Требуется Node.js 20+ и Angular CLI 20 (устанавливается как dev-зависимость).

```bash
npm install        # установка зависимостей
npm start          # ng serve → http://localhost:4200
npm run build      # production-сборка в dist/
npm test           # unit-тесты (Karma/Jasmine)
```

> В демо-сборке ошибки API выключены. Чтобы увидеть состояния ошибок и кнопку «Повторить»,
> откройте любой экран с параметром `?mockError=1`, например
> `http://localhost:4200/animals?mockError=1`.

### Демо-аккаунт

| Поле  | Значение           |
| ----- | ------------------ |
| Email | `user@example.com` |
| Пароль| любой (мин. 6 симв.) |

Сессия хранится в `localStorage` (или `sessionStorage`, если снять галочку «Запомнить меня»).
`/profile` и `/apply/:animalId` защищены `authGuard`.

---

## Технологии

- Angular 20, standalone-компоненты, `loadComponent` (ленивые маршруты).
- TypeScript `strict` + `strictTemplates`, без `any`.
- Состояние — на сигналах: `signal()`, `computed()`, `toSignal()` (+ `toObservable()`).
- Новый control flow: `@if`, `@for`, `@switch`, `@defer`, `@let`.
- DI через `inject()`. Формы — Reactive Forms. Стили — SCSS поверх дизайн-токенов.
- Без Angular Material. Иконки — собственный набор на базе **Lucide** (см. ниже).

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
    ├── app.config.ts              # провайдеры: роутер + mock API (точка замены на бэкенд)
    ├── app.routes.ts              # ленивые маршруты (loadComponent)
    ├── app.ts / app.html / app.scss
    ├── core/
    │   ├── models/                # Animal, AnimalFilters, User, Application
    │   ├── tokens/                # интерфейсы API + InjectionToken'ы
    │   ├── mock/                  # mock-реализации API + данные животных
    │   ├── services/              # SessionService, FavoritesService, ToastService
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

| Путь              | Экран                                         |
| ----------------- | --------------------------------------------- |
| `/`               | Главная: hero-карусель (3 слайда) + популярные |
| `/animals`        | Каталог с фильтрами и сортировкой              |
| `/animals/:id`    | Карточка животного с галереей                  |
| `/login`          | Вход (без шапки)                               |
| `/register`       | Регистрация (без шапки)                        |
| `/apply/:animalId`| Заявка на усыновление (только для авторизованных) |
| `/profile`        | Профиль и «Мои заявки» (только для авторизованных) |
| `/favorites`      | Избранное                                      |
| `/about`          | О нас                                          |
| `/contacts`       | Контакты                                       |
| `**`              | 404 «Страница не найдена»                      |

---

## Замена mock на реальный бэкенд

Каждый API описан интерфейсом и зарегистрирован через `InjectionToken`. Чтобы перейти на реальный
HTTP, нужно **заменить один провайдер** в `src/app/app.config.ts`:

```ts
// было
{ provide: ANIMALS_API, useClass: MockAnimalsApi },
// стало
{ provide: ANIMALS_API, useClass: HttpAnimalsApi },
```

Точки замены (все помечены комментарием `// TODO(BACKEND): заменить mock на реальный API`):

| Токен             | Интерфейс (контракт)                                   | Mock-реализация                                  | Методы |
| ----------------- | ------------------------------------------------------ | ------------------------------------------------ | ------ |
| `AUTH_API`        | `src/app/core/tokens/api.tokens.ts` → `AuthApi`         | `core/mock/auth.mock.ts` → `MockAuthApi`         | `login`, `register`, `logout`, `currentUser` |
| `ANIMALS_API`     | `AnimalsApi`                                            | `core/mock/animals.mock.ts` → `MockAnimalsApi`   | `getAnimals(filters)`, `getAnimalById`, `getPopular` |
| `FAVORITES_API`   | `FavoritesApi`                                          | `core/mock/favorites.mock.ts` → `MockFavoritesApi` | `list`, `toggle`, `has` |
| `APPLICATIONS_API`| `ApplicationsApi`                                       | `core/mock/applications.mock.ts` → `MockApplicationsApi` | `create`, `getMy` |

Данные животных для мока: `src/app/core/mock/animals.data.ts` (15 животных: кошки, собаки, другие,
разные возрасты). Логика фильтрации/сортировки — `applyFilters()` в `core/models/filters.model.ts`.

Задержка и эмуляция ошибок — `src/app/core/mock/mock-utils.ts`:

```ts
export const MOCK_ERROR_RATE = 0; // поставьте 0.15, чтобы изредка видеть ошибки
// либо откройте приложение с ?mockError=1 — тогда ошибки будут всегда
```

### Изображения

Фото животных и иллюстрации — заглушки `picsum.photos` с фиксированными seed'ами. В коде помечены
`// TODO(IMAGES): заменить на реальные фото из API`. Замена — в `core/mock/animals.data.ts`
(функция `gallery`), а также в баннерах `home`, `about`, `auth-layout`.

Чтобы интерфейс не показывал «сломанные» картинки, если внешний хост недоступен (офлайн, прокси,
блокировка), все `<img>` помечены директивой `appImgFallback`
(`shared/ui/img-fallback/img-fallback.directive.ts`): при ошибке загрузки подставляется локальная
SVG-заглушка (data URI, без сети). Когда подключите реальные фото, директиву можно оставить — она
будет страховать от битых ссылок.

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

## Принятые решения (чего не было в ТЗ)

1. **Иконки.** Используются официальные иконки из пакета `lucide-angular`, но с локальным резолвом
   вместо DI-токена `LUCIDE_ICONS`: набор `APP_ICONS` (`shared/ui/icon/icon-set.ts`) импортирует
   нужные иконки, а обёртка `IconComponent` (`shared/ui/icon/icon.ts`) выбирает иконку по kebab-имени
   и рендерит её через `<lucide-icon [img]>`. Так сохраняется единый линейный стиль Lucide,
   tree-shaking и не возникает конфликтов с multi-провайдерами. Иконки адресуются по имени:
   `<app-icon name="paw-print" [size]="20" />`.
- **Поиск в шапке.** В эталоне у иконки поиска нет отдельного экрана, поэтому она раскрывает
  строку поиска и ведёт в каталог с параметром `?q=`. Каталог фильтрует по кличке/породе
  (`AnimalFilters.search`).
- **Экран «Контакты».** Пункт меню есть в эталоне, но описание экрана в задании отсутствовало —
  сделана простая страница с контактами и ссылкой в каталог.
- **Подвал.** Добавлен лёгкий footer на основных страницах (не на экранах входа/регистрации).
  Карточки «Популярные животные» — в 5 колонок, на планшете/мобильных переходят в 3/2/1.
- **Сердце-избранное.** Активное состояние окрашено в `--color-primary` (единственный акцент
  палитры), а не в красный, чтобы не вводить новый цвет вне токенов.
- **«Забыли пароль?»** Ведёт в toast-заглушку «появится в следующей версии» — бэкенда нет.
- **Редактирование профиля** сохраняется локально в `SessionService` (без бэкенда).
- **Эмуляция ошибок.** Чтобы демонстрация была стабильной, по умолчанию `MOCK_ERROR_RATE = 0`;
  ошибки включаются флагом `?mockError=1` в URL.
- **Заглушка для фото.** Все `<img>` используют директиву `appImgFallback`: если внешний хост с
  фото недоступен, подставляется локальная SVG-заглушка — интерфейс не показывает «сломанные»
  картинки.
- **Выход из аккаунта** очищает сессию и возвращает на главную (страница `/profile` защищена
  гардом, поэтому оставаться на ней после выхода некорректно).
- **Сортировка каталога** вынесена в отдельный `FormControl` (а не в форму фильтров), потому что
  она находится вне блока фильтров и не должна сбрасываться вместе с ними.

---

## Проверка качества

- `ng build` проходит без ошибок и предупреждений (production).
- Строгий TypeScript, без `any`; `strictTemplates` включён.
- Все списки имеют состояния: skeleton → ошибка с кнопкой «Повторить» → пустое состояние.
- Формы: валидация, блокировка кнопки при невалидности/отправке, ошибки под полями, серверные
  ошибки, успех — через toast.
- Адаптив: сетка каталога 3 → 2 → 1, мобильная панель фильтров, бургер-меню в шапке.
- Доступность: семантические теги, `label`, `aria-label`/`aria-pressed`/`aria-expanded` на
  иконочных кнопках, видимые `:focus-visible`.
- В коде нет `console.log`.
