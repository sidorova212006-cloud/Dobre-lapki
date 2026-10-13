import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

/** Плейсхолдер загрузки. */
@Component({
  selector: 'app-skeleton',
  template: `<span class="skeleton" [style]="style()"></span>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SkeletonComponent {
  readonly width = input<string>('100%');
  readonly height = input<string>('16px');
  readonly radius = input<string>('var(--radius-sm)');

  readonly style = computed(
    () => `width:${this.width()};height:${this.height()};border-radius:${this.radius()};`,
  );
}
