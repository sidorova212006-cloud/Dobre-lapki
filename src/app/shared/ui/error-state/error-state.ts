import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { IconComponent } from '../icon/icon';

/** Состояние ошибки списка с кнопкой повтора. */
@Component({
  selector: 'app-error-state',
  imports: [IconComponent],
  template: `
    <div class="error-state" role="alert">
      <span class="error-state__icon"><app-icon name="circle-alert" [size]="28" /></span>
      <h3 class="error-state__title">{{ title() }}</h3>
      <p class="error-state__text">{{ message() }}</p>
      <button type="button" class="btn btn--secondary" (click)="retry.emit()">
        <span class="btn__icon"><app-icon name="refresh-cw" [size]="16" /></span>
        Повторить
      </button>
    </div>
  `,
  styles: `
    .error-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      gap: var(--space-3);
      padding: var(--space-12) var(--space-6);
      background: var(--color-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
    }
    .error-state__icon {
      display: grid;
      place-items: center;
      width: 60px;
      height: 60px;
      border-radius: var(--radius-pill);
      background: var(--color-danger-soft);
      color: var(--color-danger);
    }
    .error-state__text {
      max-width: 420px;
      color: var(--color-text-muted);
      font-size: 14px;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ErrorStateComponent {
  readonly title = input<string>('Что-то пошло не так');
  readonly message = input<string>('Не удалось загрузить данные.');
  readonly retry = output<void>();
}
