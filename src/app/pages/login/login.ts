import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { merge, startWith } from 'rxjs';

import { SessionService } from '../../core/services/session.service';
import { ToastService } from '../../core/services/toast.service';
import { errorMessage } from '../../core/utils/async-state';
import { controlError } from '../../core/utils/form-errors';
import { AuthLayoutComponent } from '../../layout/auth-layout/auth-layout';
import { ButtonComponent } from '../../shared/ui/button/button';
import { CheckboxComponent } from '../../shared/ui/checkbox/checkbox';
import { IconComponent } from '../../shared/ui/icon/icon';
import { InputComponent } from '../../shared/ui/input/input';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    AuthLayoutComponent,
    ButtonComponent,
    CheckboxComponent,
    IconComponent,
    InputComponent,
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginPage {
  private readonly session = inject(SessionService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected readonly form = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(6)],
    }),
    remember: new FormControl(true, { nonNullable: true }),
  });

  /** Реактивный тик для пересчёта ошибок полей. */
  private readonly formTick = toSignal(
    merge(this.form.statusChanges, this.form.valueChanges).pipe(startWith(null)),
    { initialValue: null },
  );

  protected readonly submitting = signal(false);
  protected readonly submitted = signal(false);
  protected readonly serverError = signal('');

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
      { required: 'Введите пароль', minlength: 'Минимум 6 символов' },
      this.submitted(),
    );
  });

  protected submit(): void {
    this.submitted.set(true);
    if (this.form.invalid || this.submitting()) return;

    this.submitting.set(true);
    this.serverError.set('');
    const value = this.form.getRawValue();

    this.session.login({ email: value.email, password: value.password, remember: this.form.controls.remember.value }).subscribe({
      next: (user) => {
        this.submitting.set(false);
        this.toast.success(`Добро пожаловать, ${user.name}!`);
        const redirect = this.route.snapshot.queryParamMap.get('redirect') ?? '/profile';
        void this.router.navigateByUrl(redirect);
      },
      error: (error: unknown) => {
        this.submitting.set(false);
        this.serverError.set(errorMessage(error));
      },
    });
  }

  protected forgotPassword(): void {
    this.toast.info('Восстановление пароля появится в следующей версии');
  }
}
