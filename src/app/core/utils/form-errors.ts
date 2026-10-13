import { AbstractControl } from '@angular/forms';

/**
 * Возвращает текст ошибки поля. Ошибка показывается, только если поле
 * уже изменялось/теряло фокус или была попытка отправки формы.
 */
export function controlError(
  control: AbstractControl,
  messages: Record<string, string>,
  forceShow = false,
): string | null {
  if (!control.invalid) return null;
  if (!forceShow && !control.dirty && !control.touched) return null;

  const errors = control.errors ?? {};
  for (const key of Object.keys(errors)) {
    const message = messages[key];
    if (message) return message;
  }
  return Object.values(messages)[0] ?? 'Некорректное значение';
}
