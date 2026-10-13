import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { LucideAngularModule, LucideIconData } from 'lucide-angular';
import { APP_ICONS } from './icon-set';

/** 'chevron-down' → 'ChevronDown' (имена, экспортируемые lucide-angular). */
function toPascalCase(value: string): string {
  return value
    .split(/[-_\s]+/)
    .filter(Boolean)
    .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
    .join('');
}

/**
 * Обёртка над Lucide: единый линейный стиль иконок во всём приложении.
 * Данные иконки берутся из локального набора APP_ICONS (tree-shaking).
 *
 * <app-icon name="paw-print" [size]="20" />
 */
@Component({
  selector: 'app-icon',
  imports: [LucideAngularModule],
  template: `<lucide-icon [img]="data()" [size]="size()" [strokeWidth]="strokeWidth()" />`,
  styles: `
    :host {
      display: inline-flex;
      line-height: 0;
      color: inherit;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class IconComponent {
  readonly name = input.required<string>();
  readonly size = input<number>(20);
  readonly strokeWidth = input<number>(1.75);

  protected readonly data = computed<LucideIconData>(() => {
    const key = toPascalCase(this.name()) as keyof typeof APP_ICONS;
    return APP_ICONS[key] ?? APP_ICONS.PawPrint;
  });
}
