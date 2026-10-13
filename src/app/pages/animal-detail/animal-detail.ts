import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';

import { ANIMALS_API } from '../../core/tokens/injection-tokens';
import {
  Animal,
  ANIMAL_GENDER_LABEL,
  ANIMAL_TYPE_LABEL,
  ageLabel,
} from '../../core/models/animal.model';
import { asyncState } from '../../core/utils/async-state';
import { FavoritesService } from '../../core/services/favorites.service';
import { BadgeComponent } from '../../shared/ui/badge/badge';
import { ButtonComponent } from '../../shared/ui/button/button';
import { ErrorStateComponent } from '../../shared/ui/error-state/error-state';
import { HeartButtonComponent } from '../../shared/ui/heart-button/heart-button';
import { IconComponent } from '../../shared/ui/icon/icon';
import { ImgFallbackDirective } from '../../shared/ui/img-fallback/img-fallback.directive';
import { SkeletonComponent } from '../../shared/ui/skeleton/skeleton';

interface Characteristic {
  icon: string;
  label: string;
  value: string;
}

@Component({
  selector: 'app-animal-detail',
  imports: [
    RouterLink,
    BadgeComponent,
    ButtonComponent,
    ErrorStateComponent,
    HeartButtonComponent,
    IconComponent,
    ImgFallbackDirective,
    SkeletonComponent,
  ],
  templateUrl: './animal-detail.html',
  styleUrl: './animal-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AnimalDetailPage {
  private readonly animalsApi = inject(ANIMALS_API);
  private readonly favorites = inject(FavoritesService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  private readonly animalId = toSignal(
    this.route.paramMap.pipe(map((params) => Number(params.get('id')))),
    { initialValue: Number(this.route.snapshot.paramMap.get('id')) },
  );

  protected readonly selected = signal(0);

  protected readonly animal = asyncState<Animal>(() => this.animalsApi.getAnimalById(this.animalId()));

  protected readonly animalData = computed(() => {
    const state = this.animal.state();
    return state.status === 'success' ? state.data : null;
  });
  protected readonly animalError = computed(() => {
    const state = this.animal.state();
    return state.status === 'error' ? state.error : '';
  });
  protected readonly animalLoading = computed(() => this.animal.state().status === 'loading');

  protected readonly isFavorite = computed(() => this.favorites.isFavorite(this.animalId()));

  protected readonly mainPhoto = computed(() => {
    const item = this.animalData();
    if (!item) return '';
    return item.photos[this.selected()] ?? item.photos[0] ?? '';
  });

  protected readonly meta = computed(() => {
    const item = this.animalData();
    if (!item) return '';
    return `${ageLabel(item.ageYears)} • ${ANIMAL_TYPE_LABEL[item.type]} • ${
      ANIMAL_GENDER_LABEL[item.gender]
    }`;
  });

  protected readonly characteristics = computed<Characteristic[]>(() => {
    const item = this.animalData();
    if (!item) return [];
    return [
      { icon: 'stethoscope', label: 'Здоровье', value: item.health },
      { icon: 'shield-check', label: 'Стерилизация', value: item.sterilized ? 'да' : 'нет' },
      { icon: 'syringe', label: 'Прививок', value: item.vaccinated ? 'да' : 'нет' },
    ];
  });

  constructor() {
    this.favorites.ensureLoaded();
  }

  protected selectPhoto(index: number): void {
    this.selected.set(index);
  }

  protected toggleFavorite(): void {
    this.favorites.toggle(this.animalId());
  }

  protected openApplication(): void {
    void this.router.navigate(['/apply', this.animalId()]);
  }
}
