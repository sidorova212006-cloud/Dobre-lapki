import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ApplicationStatus, APPLICATION_STATUS_LABEL } from '../../../core/models/application.model';

export type BadgeTone = ApplicationStatus | 'soft' | 'neutral';

/** Пилюля статуса: мягкий фон + соответствующий цвет текста. */
@Component({
  selector: 'app-badge',
  template: `<span [class]="'badge badge--' + tone()">{{ label() }}</span>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BadgeComponent {
  readonly tone = input<BadgeTone>('soft');
  readonly text = input<string | null>(null);

  readonly label = computed(() => {
    const explicit = this.text();
    if (explicit) return explicit;
    const tone = this.tone();
    if (tone === 'pending' || tone === 'approved' || tone === 'rejected') {
      return APPLICATION_STATUS_LABEL[tone];
    }
    return '';
  });
}
