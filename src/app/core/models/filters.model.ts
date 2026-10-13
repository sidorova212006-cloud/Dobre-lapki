import { Animal, AnimalGender, AnimalType } from './animal.model';

/** Возрастные диапазоны фильтра каталога. */
export type AgeBucket = 'under1' | 'from1to3' | 'over3';

export type AnimalSort = 'newest' | 'oldest' | 'name';

export interface AnimalFilters {
  types: AnimalType[];
  genders: AnimalGender[];
  ages: AgeBucket[];
  sort: AnimalSort;
  /** Поиск по кличке или породе (поиск из шапки). */
  search?: string;
}

export const DEFAULT_FILTERS: AnimalFilters = {
  types: [],
  genders: [],
  ages: [],
  sort: 'newest',
};

export function emptyFilters(): AnimalFilters {
  return { types: [], genders: [], ages: [], sort: 'newest' };
}

export function ageBucketOf(animal: Animal): AgeBucket {
  if (animal.ageYears < 1) return 'under1';
  if (animal.ageYears <= 3) return 'from1to3';
  return 'over3';
}

/** Единая функция фильтрации/сортировки — используется и в моках, и в UI. */
export function applyFilters(animals: readonly Animal[], filters: Partial<AnimalFilters>): Animal[] {
  const types = filters.types ?? [];
  const genders = filters.genders ?? [];
  const ages = filters.ages ?? [];
  const search = filters.search?.trim().toLowerCase() ?? '';

  const result = animals.filter((animal) => {
    if (types.length > 0 && !types.includes(animal.type)) return false;
    if (genders.length > 0 && !genders.includes(animal.gender)) return false;
    if (ages.length > 0 && !ages.includes(ageBucketOf(animal))) return false;
    if (search && !`${animal.name} ${animal.breed}`.toLowerCase().includes(search)) return false;
    return true;
  });

  const sort: AnimalSort = filters.sort ?? 'newest';
  return [...result].sort((a, b) => {
    switch (sort) {
      case 'oldest':
        return a.arrivedAt.localeCompare(b.arrivedAt);
      case 'name':
        return a.name.localeCompare(b.name, 'ru');
      case 'newest':
      default:
        return b.arrivedAt.localeCompare(a.arrivedAt);
    }
  });
}
