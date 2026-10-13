import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ToastService, ToastKind } from '../../../core/services/toast.service';
import { IconComponent } from '../icon/icon';

@Component({
  selector: 'app-toast-host',
  imports: [IconComponent],
  template: `
    <div class="toasts" role="status" aria-live="polite">
      @for (toast of toasts(); track toast.id) {
        <div class="toast" [class]="'toast toast--' + toast.kind">
          <span class="toast__icon"><app-icon [name]="iconFor(toast.kind)" [size]="18" /></span>
          <span class="toast__text">{{ toast.text }}</span>
          <button
            type="button"
            class="toast__close"
            aria-label="Закрыть уведомление"
            (click)="dismiss(toast.id)"
          >
            <app-icon name="x" [size]="16" />
          </button>
        </div>
      }
    </div>
  `,
  styles: `
    .toasts {
      position: fixed;
      right: var(--space-6);
      bottom: var(--space-6);
      z-index: 1000;
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
      max-width: min(360px, calc(100vw - 32px));
    }
    .toast {
      display: flex;
      align-items: center;
      gap: var(--space-3);
      padding: 12px 14px;
      background: var(--color-surface);
      border: 1px solid var(--color-border);
      border-left-width: 3px;
      border-radius: var(--radius-sm);
      box-shadow: var(--shadow-pop);
      font-size: 14px;
      font-weight: 500;
      animation: toast-in 0.2s ease;
    }
    .toast--success {
      border-left-color: var(--color-primary);
    }
    .toast--success .toast__icon {
      color: var(--color-primary);
    }
    .toast--error {
      border-left-color: var(--color-danger);
    }
    .toast--error .toast__icon {
      color: var(--color-danger);
    }
    .toast--info {
      border-left-color: var(--color-text-muted);
    }
    .toast__icon {
      display: inline-flex;
      line-height: 0;
      flex: none;
    }
    .toast__text {
      flex: 1;
    }
    .toast__close {
      display: inline-flex;
      border: 0;
      background: transparent;
      color: var(--color-text-muted);
      cursor: pointer;
      padding: 2px;
      line-height: 0;
      border-radius: 6px;
    }
    .toast__close:hover {
      color: var(--color-text);
    }
    @keyframes toast-in {
      from {
        opacity: 0;
        transform: translateY(8px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }
    @media (prefers-reduced-motion: reduce) {
      .toast {
        animation: none;
      }
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToastHostComponent {
  private readonly service = inject(ToastService);
  protected readonly toasts = this.service.toasts;

  protected dismiss(id: number): void {
    this.service.dismiss(id);
  }

  protected iconFor(kind: ToastKind): string {
    if (kind === 'success') return 'circle-check';
    if (kind === 'error') return 'circle-alert';
    return 'info';
  }
}
