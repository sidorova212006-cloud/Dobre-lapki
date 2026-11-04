import { AnimalGender, AnimalType } from './animal.model';

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
