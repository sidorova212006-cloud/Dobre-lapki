export type AnimalType = 'cat' | 'dog' | 'other';
export type AnimalGender = 'male' | 'female';

export const ANIMAL_TYPE_LABEL: Record<AnimalType, string> = {
  cat: 'Кошка',
  dog: 'Собака',
  other: 'Другое',
};

export const ANIMAL_GENDER_LABEL: Record<AnimalGender, string> = {
  male: 'Мальчик',
  female: 'Девочка',
};

/** Плюрализация возраста: 1 год • 2 года • 5 лет. */
export function ageLabel(ageYears: number): string {
  if (ageYears < 1) {
    const months = Math.max(1, Math.round(ageYears * 12));
    return `${months} ${plural(months, 'месяц', 'месяца', 'месяцев')}`;
  }
  const years = Math.floor(ageYears);
  return `${years} ${plural(years, 'год', 'года', 'лет')}`;
}

function plural(n: number, one: string, few: string, many: string): string {
  const mod10 = n % 10;
  const mod100 = n % 100;
  if (mod10 === 1 && mod100 !== 11) return one;
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20)) return few;
  return many;
}

export interface Animal {
  id: number;
  name: string;
  type: AnimalType;
  gender: AnimalGender;
  ageYears: number;
  breed: string;
  description: string;
  /** TODO(IMAGES): заменить на реальные фото из API */
  photos: string[];
  health: string;
  sterilized: boolean;
  vaccinated: boolean;
  /** Дата поступления в приют (ISO) — используется для сортировки «Сначала новые». */
  arrivedAt: string;
}
