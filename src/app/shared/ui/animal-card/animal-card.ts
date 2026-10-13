import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Animal, ANIMAL_TYPE_LABEL, ageLabel } from '../../../core/models/animal.model';
import { HeartButtonComponent } from '../heart-button/heart-button';
import { ImgFallbackDirective } from '../img-fallback/img-fallback.directive';

@Component({
  selector: 'app-animal-card',
  imports: [RouterLink, HeartButtonComponent, ImgFallbackDirective],
  template: `
    <article class="animal-card">
      <div class="animal-card__media">
        <a
          class="animal-card__link"
          [routerLink]="['/animals', animal().id]"
          [attr.aria-label]="'Подробнее о ' + animal().name"
        >
          <img [src]="photo()" [alt]="animal().name" loading="lazy" appImgFallback />
        </a>
        <div class="animal-card__fav">
          <app-heart-button [animalId]="animal().id" />
        </div>
      </div>
      <div class="animal-card__body">
        <a class="animal-card__name" [routerLink]="['/animals', animal().id]">{{ animal().name }}</a>
        <span class="animal-card__meta">{{ meta() }}</span>
      </div>
    </article>
  `,
  styles: `
    .animal-card__link {
      display: block;
      width: 100%;
      height: 100%;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AnimalCardComponent {
  readonly animal = input.required<Animal>();

  /** TODO(IMAGES): заменить на реальные фото из API. */
  protected readonly photo = computed(
    () => this.animal().photos[0] ?? 'https://picsum.photos/seed/dp-placeholder/800/600',
  );

  protected readonly meta = computed(() => {
    const item = this.animal();
    return `${ageLabel(item.ageYears)} • ${ANIMAL_TYPE_LABEL[item.type]}`;
  });
}
