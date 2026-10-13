import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ButtonComponent } from '../../shared/ui/button/button';
import { IconComponent } from '../../shared/ui/icon/icon';

@Component({
  selector: 'app-not-found',
  imports: [ButtonComponent, IconComponent],
  template: `
    <div class="page">
      <div class="container">
        <div class="notfound">
          <span class="notfound__icon"><app-icon name="paw-print" [size]="34" /></span>
          <p class="notfound__code">404</p>
          <h1 class="notfound__title">Страница не найдена</h1>
          <p class="notfound__text">
            Похоже, эта страница убежала вместе с котёнком. Вернёмся на главную?
          </p>
          <app-button size="lg" routerLink="/">На главную</app-button>
        </div>
      </div>
    </div>
  `,
  styles: `
    .notfound {
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      gap: var(--space-3);
      padding: var(--space-16) var(--space-6);
      background: var(--color-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
    }
    .notfound__icon {
      display: grid;
      place-items: center;
      width: 72px;
      height: 72px;
      border-radius: var(--radius-pill);
      background: var(--color-primary-soft);
      color: var(--color-primary);
    }
    .notfound__code {
      color: var(--color-primary);
      font-size: 15px;
      font-weight: 800;
      letter-spacing: 0.12em;
    }
    .notfound__title {
      font-size: 30px;
      font-weight: 800;
      letter-spacing: -0.02em;
    }
    .notfound__text {
      max-width: 44ch;
      color: var(--color-text-muted);
      font-size: 15px;
      margin-bottom: var(--space-3);
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NotFoundPage {}
