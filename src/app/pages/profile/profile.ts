import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { merge, map, startWith } from 'rxjs';

import { ApplicationsService } from '../../core/services/applications.service';
import { Application } from '../../core/models/application.model';
import { asyncState, errorMessage } from '../../core/utils/async-state';
import { controlError } from '../../core/utils/form-errors';
import { SessionService } from '../../core/services/session.service';
import { FavoritesService } from '../../core/services/favorites.service';
import { ToastService } from '../../core/services/toast.service';
import { BadgeComponent } from '../../shared/ui/badge/badge';
import { ButtonComponent } from '../../shared/ui/button/button';
import { EmptyStateComponent } from '../../shared/ui/empty-state/empty-state';
import { ErrorStateComponent } from '../../shared/ui/error-state/error-state';
import { IconComponent } from '../../shared/ui/icon/icon';
import { ImgFallbackDirective } from '../../shared/ui/img-fallback/img-fallback.directive';
import { InputComponent } from '../../shared/ui/input/input';
import { SkeletonComponent } from '../../shared/ui/skeleton/skeleton';

type MenuKey = 'profile' | 'applications';

@Component({
  selector: 'app-profile',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    BadgeComponent,
    ButtonComponent,
    EmptyStateComponent,
    ErrorStateComponent,
    IconComponent,
    ImgFallbackDirective,
    InputComponent,
    SkeletonComponent,
  ],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfilePage {
  private readonly session = inject(SessionService);
  private readonly favorites = inject(FavoritesService);
  private readonly applicationsApi = inject(ApplicationsService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  protected readonly user = this.session.user;
  protected readonly activeMenu = signal<MenuKey>('profile');
  protected readonly editing = signal(false);

  protected readonly form = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(2)],
    }),
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    phone: new FormControl('', { nonNullable: true }),
  });

  private readonly formTick = toSignal(
    merge(this.form.statusChanges, this.form.valueChanges).pipe(startWith(null)),
    { initialValue: null },
  );

  protected readonly nameError = computed<string | null>(() => {
    this.formTick();
    return controlError(this.form.controls.name, {
      required: 'Укажите имя',
      minlength: 'Слишком короткое имя',
    });
  });

  protected readonly emailError = computed<string | null>(() => {
    this.formTick();
    return controlError(this.form.controls.email, {
      required: 'Укажите email',
      email: 'Введите корректный email',
    });
  });

  protected readonly applications = asyncState<Application[]>(() =>
    this.applicationsApi.getMy().pipe(map((page) => page.items)),
  );
  protected readonly applicationsData = computed(() => {
    const state = this.applications.state();
    return state.status === 'success' ? state.data : null;
  });
  protected readonly applicationsError = computed(() => {
    const state = this.applications.state();
    return state.status === 'error' ? state.error : '';
  });
  protected readonly applicationsLoading = computed(
    () => this.applications.state().status === 'loading',
  );

  protected readonly saving = signal(false);
  protected readonly saveError = signal('');

  constructor() {
    const user = this.session.user();
    if (user) {
      this.form.patchValue({ name: user.name, email: user.email, phone: user.phone });
    }
  }

  protected selectMenu(key: MenuKey): void {
    this.activeMenu.set(key);
    const id = key === 'profile' ? 'profile-section' : 'applications-section';
    if (typeof document !== 'undefined') {
      document.getElementById(id)?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  protected startEdit(): void {
    const user = this.session.user();
    if (user) {
      this.form.patchValue({ name: user.name, email: user.email, phone: user.phone });
    }
    this.editing.set(true);
  }

  protected cancelEdit(): void {
    this.editing.set(false);
    const user = this.session.user();
    if (user) {
      this.form.patchValue({ name: user.name, email: user.email, phone: user.phone });
    }
  }

  protected save(): void {
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.saving.set(true);
    this.saveError.set('');

    this.session
      .updateProfile({ name: value.name, email: value.email, phone: value.phone })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.editing.set(false);
          this.toast.success('Профиль обновлён');
        },
        error: (error: unknown) => {
          this.saving.set(false);
          this.saveError.set(errorMessage(error));
          this.toast.error(errorMessage(error));
        },
      });
  }

  protected openFavorites(): void {
    this.favorites.ensureLoaded();
  }

  protected logout(): void {
    this.session.logout();
    void this.router.navigateByUrl('/');
  }

  protected formatDate(iso: string): string {
    return new Intl.DateTimeFormat('ru-RU', {
      day: 'numeric',
      month: 'long',
      year: 'numeric',
    }).format(new Date(iso));
  }
}
