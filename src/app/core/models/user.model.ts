export interface User {
  id: number;
  name: string;
  email: string;
  phone: string;
  /** TODO(IMAGES): avatar URL */
  avatar: string;
}

export interface LoginDto {
  email: string;
  password: string;
  remember?: boolean;
}

export interface RegisterDto {
  name: string;
  email: string;
  phone?: string;
  password: string;
}
