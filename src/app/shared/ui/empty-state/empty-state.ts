import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { IconComponent } from '../icon/icon';

/** Пустое состояние списка. Проецирует кнопки/ссылки через ng-content. */
@Component({
  selector: 'app-empty-state',
  imports: [IconComponent],
  template: `
    <div class="empty">
      <span class="empty__icon"><app-icon [name]="icon()" [size]="30" /></span>
      <h3 class="empty__title">{{ title() }}</h3>
      @if (text()) {
        <p class="empty__text">{{ text() }}</p>
      }
      <div class="empty__actions"><ng-content /></div>
    </div>
  `,
  styles: `
    .empty {
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      gap: var(--space-3);
      padding: var(--space-12) var(--space-6);
      background: var(--color-surface);
      border: 1px dashed var(--color-border);
      border-radius: var(--radius-md);
    }
    .empty__icon {
      display: grid;
      place-items: center;
      width: 64px;
      height: 64px;
      border-radius: var(--radius-pill);
      background: var(--color-primary-soft);
      color: var(--color-primary);
    }
    .empty__title {
      font-size: 18px;
    }
    .empty__text {
      max-width: 380px;
      color: var(--color-text-muted);
      font-size: 14px;
    }
    .empty__actions {
      margin-top: var(--space-2);
      display: flex;
      gap: var(--space-3);
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmptyStateComponent {
  readonly icon = input<string>('paw-print');
  readonly title = input.required<string>();
  readonly text = input<string>('');
}
