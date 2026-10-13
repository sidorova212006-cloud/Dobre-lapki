import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { merge, startWith } from 'rxjs';

import { SessionService } from '../../core/services/session.service';
import { ToastService } from '../../core/services/toast.service';
import { errorMessage } from '../../core/utils/async-state';
import { controlError } from '../../core/utils/form-errors';
import { AuthLayoutComponent } from '../../layout/auth-layout/auth-layout';
import { ButtonComponent } from '../../shared/ui/button/button';
import { IconComponent } from '../../shared/ui/icon/icon';
import { InputComponent } from '../../shared/ui/input/input';

const passwordsMatch: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
  const password = group.get('password')?.value as string | undefined;
  const confirm = group.get('confirm')?.value as string | undefined;
  if (password && confirm && password !== confirm) {
    return { mismatch: true };
  }
  return null;
};

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink, AuthLayoutComponent, ButtonComponent, IconComponent, InputComponent],
  templateUrl: './register.html',
  styleUrl: './register.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegisterPage {
  private readonly session = inject(SessionService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  protected readonly form = new FormGroup(
    {
      name: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.minLength(2)],
      }),
      email: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.email],
      }),
      password: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.minLength(6)],
      }),
      confirm: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
    },
    { validators: passwordsMatch },
  );

  private readonly formTick = toSignal(
    merge(this.form.statusChanges, this.form.valueChanges).pipe(startWith(null)),
    { initialValue: null },
  );

  protected readonly submitting = signal(false);
  protected readonly submitted = signal(false);
  protected readonly serverError = signal('');

  protected readonly nameError = computed<string | null>(() => {
    this.formTick();
    return controlError(
      this.form.controls.name,
      { required: 'Укажите имя', minlength: 'Слишком короткое имя' },
      this.submitted(),
    );
  });

  protected readonly emailError = computed<string | null>(() => {
    this.formTick();
    return controlError(
      this.form.controls.email,
      { required: 'Укажите email', email: 'Введите корректный email' },
      this.submitted(),
    );
  });

  protected readonly passwordError = computed<string | null>(() => {
    this.formTick();
    return controlError(
      this.form.controls.password,
      { required: 'Придумайте пароль', minlength: 'Минимум 6 символов' },
      this.submitted(),
    );
  });

  protected readonly confirmError = computed<string | null>(() => {
    this.formTick();
    const control = this.form.controls.confirm;
    const show = this.submitted();
    if (this.form.hasError('mismatch') && (control.dirty || control.touched || show)) {
      return 'Пароли не совпадают';
    }
    return controlError(control, { required: 'Повторите пароль' }, this.submitted());
  });

  protected submit(): void {
    this.submitted.set(true);
    if (this.form.invalid || this.submitting()) return;

    this.submitting.set(true);
    this.serverError.set('');

    const value = this.form.getRawValue();
    this.session
      .register({ name: value.name, email: value.email, password: value.password })
      .subscribe({
        next: (user) => {
          this.submitting.set(false);
          this.toast.success(`Аккаунт создан. Добро пожаловать, ${user.name}!`);
          void this.router.navigate(['/profile']);
        },
        error: (error: unknown) => {
          this.submitting.set(false);
          this.serverError.set(errorMessage(error));
        },
      });
  }
}
