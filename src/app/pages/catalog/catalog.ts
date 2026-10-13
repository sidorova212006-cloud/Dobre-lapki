import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { distinctUntilChanged, map, skip } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

import { ANIMALS_API } from '../../core/tokens/injection-tokens';
import { Animal, AnimalGender, AnimalType } from '../../core/models/animal.model';
import { AgeBucket, AnimalFilters, AnimalSort } from '../../core/models/filters.model';
import { asyncState } from '../../core/utils/async-state';
import { AnimalCardComponent } from '../../shared/ui/animal-card/animal-card';
import { ButtonComponent } from '../../shared/ui/button/button';
import { CheckboxComponent } from '../../shared/ui/checkbox/checkbox';
import { EmptyStateComponent } from '../../shared/ui/empty-state/empty-state';
import { ErrorStateComponent } from '../../shared/ui/error-state/error-state';
import { IconComponent } from '../../shared/ui/icon/icon';
import { SelectComponent, SelectOption } from '../../shared/ui/select/select';
import { SkeletonComponent } from '../../shared/ui/skeleton/skeleton';

type FilterGroup = FormGroup<{
  cat: FormControl<boolean>;
  dog: FormControl<boolean>;
  other: FormControl<boolean>;
}>;
type GenderGroup = FormGroup<{
  male: FormControl<boolean>;
  female: FormControl<boolean>;
}>;
type AgeGroup = FormGroup<{
  under1: FormControl<boolean>;
  from1to3: FormControl<boolean>;
  over3: FormControl<boolean>;
}>;

@Component({
  selector: 'app-catalog',
  imports: [
    ReactiveFormsModule,
    AnimalCardComponent,
    ButtonComponent,
    CheckboxComponent,
    EmptyStateComponent,
    ErrorStateComponent,
    IconComponent,
    SelectComponent,
    SkeletonComponent,
  ],
  templateUrl: './catalog.html',
  styleUrl: './catalog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CatalogPage {
  private readonly animalsApi = inject(ANIMALS_API);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly filtersForm = new FormGroup({
    types: new FormGroup({
      cat: new FormControl(false, { nonNullable: true }),
      dog: new FormControl(false, { nonNullable: true }),
      other: new FormControl(false, { nonNullable: true }),
    }) as FilterGroup,
    genders: new FormGroup({
      male: new FormControl(false, { nonNullable: true }),
      female: new FormControl(false, { nonNullable: true }),
    }) as GenderGroup,
    ages: new FormGroup({
      under1: new FormControl(false, { nonNullable: true }),
      from1to3: new FormControl(false, { nonNullable: true }),
      over3: new FormControl(false, { nonNullable: true }),
    }) as AgeGroup,
  });

  /** Сортировка живёт вне формы фильтров, поэтому это отдельный контрол. */
  protected readonly sortControl = new FormControl<AnimalSort>('newest', { nonNullable: true });

  protected readonly sortOptions: readonly SelectOption[] = [
    { value: 'newest', label: 'Сначала новые' },
    { value: 'oldest', label: 'Сначала старые' },
    { value: 'name', label: 'По имени (А–Я)' },
  ];

  protected readonly search = toSignal(
    this.route.queryParamMap.pipe(map((params) => params.get('q') ?? '')),
    { initialValue: '' },
  );

  private readonly applied = signal<Partial<AnimalFilters>>({ sort: 'newest' });
  protected readonly filtersOpen = signal(false);
  protected readonly skeletons = [0, 1, 2, 3, 4, 5];

  protected readonly result = asyncState<Animal[]>(() => {
    const filters = this.applied();
    const search = this.search().trim();
    return this.animalsApi.getAnimals(search ? { ...filters, search } : filters);
  });

  protected readonly found = computed(() => {
    const state = this.result.state();
    return state.status === 'success' ? state.data.length : null;
  });

  protected readonly catalogAnimals = computed(() => {
    const state = this.result.state();
    return state.status === 'success' ? state.data : null;
  });
  protected readonly catalogError = computed(() => {
    const state = this.result.state();
    return state.status === 'error' ? state.error : '';
  });
  protected readonly catalogLoading = computed(() => this.result.state().status === 'loading');

  constructor() {
    // Смена поискового запроса в шапке → перезапрос.
    this.route.queryParamMap
      .pipe(
        map((params) => params.get('q') ?? ''),
        distinctUntilChanged(),
        skip(1),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => this.result.reload());

    // Изменение сортировки применяется сразу.
    this.sortControl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((sort) => {
        this.applied.update((filters) => ({ ...filters, sort }));
        this.result.reload();
      });
  }

  protected apply(): void {
    const value = this.filtersForm.getRawValue();
    const types = (['cat', 'dog', 'other'] as AnimalType[]).filter((type) => value.types[type]);
    const genders = (['male', 'female'] as AnimalGender[]).filter((g) => value.genders[g]);
    const ages = (['under1', 'from1to3', 'over3'] as AgeBucket[]).filter((age) => value.ages[age]);

    this.applied.set({ types, genders, ages, sort: this.sortControl.value });
    this.filtersOpen.set(false);
    this.result.reload();
  }

  protected reset(): void {
    this.filtersForm.reset({
      types: { cat: false, dog: false, other: false },
      genders: { male: false, female: false },
      ages: { under1: false, from1to3: false, over3: false },
    });
    this.sortControl.reset('newest');
  }

  protected toggleFilters(): void {
    this.filtersOpen.update((open) => !open);
  }

  protected clearSearch(): void {
    void this.router.navigate(['/animals']);
  }
}
