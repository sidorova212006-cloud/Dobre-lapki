import { Injectable, signal } from '@angular/core';

export type ToastKind = 'success' | 'error' | 'info';

export interface Toast {
  id: number;
  kind: ToastKind;
  text: string;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly items = signal<Toast[]>([]);
  private counter = 0;

  readonly toasts = this.items.asReadonly();

  success(text: string): void {
    this.show(text, 'success');
  }

  error(text: string): void {
    this.show(text, 'error');
  }

  info(text: string): void {
    this.show(text, 'info');
  }

  dismiss(id: number): void {
    this.items.update((list) => list.filter((toast) => toast.id !== id));
  }

  private show(text: string, kind: ToastKind): void {
    const id = ++this.counter;
    this.items.update((list) => [...list, { id, kind, text }]);
    setTimeout(() => this.dismiss(id), 3600);
  }
}
