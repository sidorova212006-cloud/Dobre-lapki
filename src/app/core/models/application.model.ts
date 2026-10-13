import { Animal } from './animal.model';

export type ApplicationStatus = 'pending' | 'approved' | 'rejected';

export interface Application {
  id: number;
  animal: Animal;
  status: ApplicationStatus;
  /** ISO-дата создания заявки. */
  createdAt: string;
}

export interface CreateApplicationDto {
  animalId: number;
  fullName: string;
  email: string;
  phone: string;
  about: string;
}

export const APPLICATION_STATUS_LABEL: Record<ApplicationStatus, string> = {
  pending: 'На рассмотрении',
  approved: 'Одобрено',
  rejected: 'Отказано',
};
