import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { interval, map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

import { ANIMALS_API } from '../../core/tokens/injection-tokens';
import { Animal } from '../../core/models/animal.model';
import { asyncState } from '../../core/utils/async-state';
import { AnimalCardComponent } from '../../shared/ui/animal-card/animal-card';
import { ButtonComponent } from '../../shared/ui/button/button';
import { ErrorStateComponent } from '../../shared/ui/error-state/error-state';
import { IconComponent } from '../../shared/ui/icon/icon';
import { ImgFallbackDirective } from '../../shared/ui/img-fallback/img-fallback.directive';
import { SkeletonComponent } from '../../shared/ui/skeleton/skeleton';
import { EmptyStateComponent } from '../../shared/ui/empty-state/empty-state';

interface HeroSlide {
  title: string;
  subtitle: string;
  image: string;
  ctaLabel: string;
}

@Component({
  selector: 'app-home',
  imports: [
    RouterLink,
    AnimalCardComponent,
    ButtonComponent,
    ErrorStateComponent,
    EmptyStateComponent,
    IconComponent,
    ImgFallbackDirective,
    SkeletonComponent,
  ],
  templateUrl: './home.html',
  styleUrl: './home.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePage {
  private readonly animalsApi = inject(ANIMALS_API);

  // TODO(IMAGES): заменить на реальные фото из API.
  protected readonly slides: readonly HeroSlide[] = [
    {
      title: 'Каждое животное заслуживает любви',
      subtitle:
        'Мы находим дом кошкам и собакам, которые остались без хозяев. Подарите питомцу вторую жизнь — и обретите верного друга.',
      image: 'https://picsum.photos/seed/dp-hero-1/1200/900',
      ctaLabel: 'Посмотреть животных',
    },
    {
      title: 'Подари тепло тому, кто ждёт',
      subtitle:
        'В нашем приюте живут десятки животных разных возрастов и характеров. Среди них обязательно есть тот, кто станет вашим.',
      image: 'https://picsum.photos/seed/dp-hero-2/1200/900',
      ctaLabel: 'Посмотреть животных',
    },
    {
      title: 'Станьте частью добрых дел',
      subtitle:
        'Одним визитом или заявкой вы помогаете животным обрести семью. Начните с небольшого шага — загляните в каталог.',
      image: 'https://picsum.photos/seed/dp-hero-3/1200/900',
      ctaLabel: 'Посмотреть животных',
    },
  ];

  private readonly autoplay = toSignal(
    interval(6000).pipe(map((tick) => tick + 1)),
    { initialValue: 0 },
  );
  private readonly offset = signal(0);

  protected readonly active = computed(() => (this.autoplay() + this.offset()) % this.slides.length);
  protected readonly currentSlide = computed(() => this.slides[this.active()] ?? this.slides[0]);

  protected readonly popular = asyncState<Animal[]>(() => this.animalsApi.getPopular());
  protected readonly popularAnimals = computed(() => {
    const state = this.popular.state();
    return state.status === 'success' ? state.data : null;
  });
  protected readonly popularError = computed(() => {
    const state = this.popular.state();
    return state.status === 'error' ? state.error : '';
  });
  protected readonly popularLoading = computed(() => this.popular.state().status === 'loading');
  protected readonly skeletons = [0, 1, 2, 3, 4];

  protected goTo(index: number): void {
    const length = this.slides.length;
    const tick = this.autoplay();
    this.offset.set((((index - tick) % length) + length) % length);
  }
}
