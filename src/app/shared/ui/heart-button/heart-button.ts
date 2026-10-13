import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { FavoritesService } from '../../../core/services/favorites.service';
import { IconComponent } from '../icon/icon';

/**
 * Кнопка-сердце. Состояние берётся из FavoritesService,
 * поэтому сердце синхронизировано в каталоге, карточке и профиле.
 */
@Component({
  selector: 'app-heart-button',
  imports: [IconComponent],
  template: `
    <button
      type="button"
      class="heart"
      [class.heart--active]="favorite()"
      [attr.aria-pressed]="favorite()"
      [attr.aria-label]="favorite() ? 'Убрать из избранного' : 'Добавить в избранное'"
      (click)="toggle($event)"
    >
      <app-icon name="heart" [size]="size()" [strokeWidth]="2" />
    </button>
  `,
  styles: `
    .heart {
      display: grid;
      place-items: center;
      width: 38px;
      height: 38px;
      border-radius: var(--radius-pill);
      border: 1px solid var(--color-border);
      background: rgba(255, 255, 255, 0.92);
      color: var(--color-text-muted);
      cursor: pointer;
      transition:
        color var(--transition),
        border-color var(--transition),
        background var(--transition);
    }
    .heart:hover {
      color: var(--color-primary);
      border-color: var(--color-primary);
    }
    .heart--active {
      color: #fff;
      background: var(--color-primary);
      border-color: var(--color-primary);
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeartButtonComponent {
  private readonly favorites = inject(FavoritesService);

  readonly animalId = input.required<number>();
  readonly size = input<number>(18);

  readonly favorite = computed(() => this.favorites.isFavorite(this.animalId()));

  protected toggle(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.favorites.ensureLoaded();
    this.favorites.toggle(this.animalId());
  }
}
