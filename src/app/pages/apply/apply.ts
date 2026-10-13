import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { map, merge, startWith } from 'rxjs';

import { APPLICATIONS_API, ANIMALS_API } from '../../core/tokens/injection-tokens';
import { Animal } from '../../core/models/animal.model';
import { asyncState, errorMessage } from '../../core/utils/async-state';
import { controlError } from '../../core/utils/form-errors';
import { SessionService } from '../../core/services/session.service';
import { ToastService } from '../../core/services/toast.service';
import { ButtonComponent } from '../../shared/ui/button/button';
import { IconComponent } from '../../shared/ui/icon/icon';
import { ImgFallbackDirective } from '../../shared/ui/img-fallback/img-fallback.directive';
import { InputComponent } from '../../shared/ui/input/input';
import { SkeletonComponent } from '../../shared/ui/skeleton/skeleton';
import { TextareaComponent } from '../../shared/ui/textarea/textarea';

@Component({
  selector: 'app-apply',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    ButtonComponent,
    IconComponent,
    ImgFallbackDirective,
    InputComponent,
    SkeletonComponent,
    TextareaComponent,
  ],
  templateUrl: './apply.html',
  styleUrl: './apply.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplyPage {
  private readonly animalsApi = inject(ANIMALS_API);
  private readonly applicationsApi = inject(APPLICATIONS_API);
  private readonly session = inject(SessionService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  private readonly animalId = toSignal(
    this.route.paramMap.pipe(map((params) => Number(params.get('animalId')))),
    { initialValue: Number(this.route.snapshot.paramMap.get('animalId')) },
  );

  protected readonly animal = asyncState<Animal>(() => this.animalsApi.getAnimalById(this.animalId()));
  protected readonly animalData = computed(() => {
    const state = this.animal.state();
    return state.status === 'success' ? state.data : null;
  });
  protected readonly animalError = computed(() => {
    const state = this.animal.state();
    return state.status === 'error' ? state.error : '';
  });
  protected readonly animalLoading = computed(() => this.animal.state().status === 'loading');

  protected readonly form = new FormGroup({
    fullName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(2)],
    }),
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    phone: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.pattern(/^[+()\d\s-]{10,}$/)],
    }),
    about: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(20)],
    }),
  });

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
      this.form.controls.fullName,
      { required: 'Укажите имя и фамилию', minlength: 'Слишком короткое имя' },
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

  protected readonly phoneError = computed<string | null>(() => {
    this.formTick();
    return controlError(
      this.form.controls.phone,
      { required: 'Укажите телефон', pattern: 'Введите корректный номер' },
      this.submitted(),
    );
  });

  protected readonly aboutError = computed<string | null>(() => {
    this.formTick();
    return controlError(
      this.form.controls.about,
      { required: 'Расскажите немного о себе', minlength: 'Минимум 20 символов' },
      this.submitted(),
    );
  });

  constructor() {
    const user = this.session.user();
    if (user) {
      this.form.patchValue({ fullName: user.name, email: user.email, phone: user.phone });
    }
  }

  protected submit(): void {
    this.submitted.set(true);
    if (this.form.invalid || this.submitting()) return;

    this.submitting.set(true);
    this.serverError.set('');

    const value = this.form.getRawValue();
    this.applicationsApi
      .create({
        animalId: this.animalId(),
        fullName: value.fullName,
        email: value.email,
        phone: value.phone,
        about: value.about,
      })
      .subscribe({
        next: () => {
          this.submitting.set(false);
          this.toast.success('Заявка отправлена! Мы свяжемся с вами.');
          void this.router.navigate(['/profile']);
        },
        error: (error: unknown) => {
          this.submitting.set(false);
          this.serverError.set(errorMessage(error));
        },
      });
  }
}
