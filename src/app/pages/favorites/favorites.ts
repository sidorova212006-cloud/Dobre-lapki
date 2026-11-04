import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { AnimalsService } from '../../core/services/animals.service';
import { Animal } from '../../core/models/animal.model';
import { PagedResult } from '../../core/models/paged-result';
import { asyncState } from '../../core/utils/async-state';
import { FavoritesService } from '../../core/services/favorites.service';
import { AnimalCardComponent } from '../../shared/ui/animal-card/animal-card';
import { ButtonComponent } from '../../shared/ui/button/button';
import { EmptyStateComponent } from '../../shared/ui/empty-state/empty-state';
import { ErrorStateComponent } from '../../shared/ui/error-state/error-state';
import { SkeletonComponent } from '../../shared/ui/skeleton/skeleton';

@Component({
  selector: 'app-favorites',
  imports: [
    AnimalCardComponent,
    ButtonComponent,
    EmptyStateComponent,
    ErrorStateComponent,
    SkeletonComponent,
  ],
  templateUrl: './favorites.html',
  styleUrl: './favorites.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FavoritesPage {
  private readonly animalsApi = inject(AnimalsService);
  private readonly favorites = inject(FavoritesService);

  protected readonly skeletons = [0, 1, 2];

  private readonly animals = asyncState<PagedResult<Animal>>(() =>
    this.animalsApi.getAnimals({}, 1, 100),
  );

  protected reload(): void {
    this.animals.reload();
    this.favorites.reload();
  }

  protected readonly data = computed(() => {
    const state = this.animals.state();
    return state.status === 'success' ? state.data.items : null;
  });
  protected readonly error = computed(() => {
    const state = this.animals.state();
    return state.status === 'error' ? state.error : '';
  });
  protected readonly loading = computed(() => this.animals.state().status === 'loading');

  protected readonly list = computed(() => {
    const animals = this.data();
    if (!animals) return null;
    const ids = this.favorites.list();
    return animals.filter((animal) => ids.includes(animal.id));
  });

  constructor() {
    this.favorites.ensureLoaded();
  }
}
