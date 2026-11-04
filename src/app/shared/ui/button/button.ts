import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';

export type ButtonVariant = 'primary' | 'secondary' | 'ghost';
export type ButtonSize = 'sm' | 'md' | 'lg';

/**
 * Кнопка-обёртка. Рендерит <a routerLink>, <a href> или <button>
 * в зависимости от входных данных, сохраняя единый вид.
 */
@Component({
  selector: 'app-button',
  imports: [RouterLink],
  template: `
    @if (routerLink(); as link) {
      <a [class]="classes()" [routerLink]="link">
        <ng-content />
      </a>
    } @else if (href(); as external) {
      <a [class]="classes()" [href]="external" target="_blank" rel="noopener">
        <ng-content />
      </a>
    } @else {
      <button
        [class]="classes()"
        [type]="type()"
        [disabled]="disabled() || loading()"
        [attr.aria-busy]="loading()"
      >
        @if (loading()) {
          <span class="btn__spinner" aria-hidden="true"></span>
        }
        <ng-content />
      </button>
    }
  `,
  styles: `
    .btn__spinner {
      display: inline-block;
      width: 16px;
      height: 16px;
      margin-right: 8px;
      vertical-align: -2px;
      border: 2px solid currentColor;
      border-top-color: transparent;
      border-radius: 50%;
      animation: btn-spin 0.7s linear infinite;
    }

    @keyframes btn-spin {
      to {
        transform: rotate(360deg);
      }
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ButtonComponent {
  readonly variant = input<ButtonVariant>('primary');
  readonly size = input<ButtonSize>('md');
  readonly full = input(false);
  readonly disabled = input(false);
  readonly loading = input(false);
  readonly type = input<'button' | 'submit'>('button');
  readonly routerLink = input<string | unknown[] | null>(null);
  readonly href = input<string | null>(null);

  readonly classes = computed(() =>
    [
      'btn',
      `btn--${this.variant()}`,
      this.size() === 'md' ? '' : `btn--${this.size()}`,
      this.full() ? 'btn--full' : '',
    ]
      .filter(Boolean)
      .join(' '),
  );
}
