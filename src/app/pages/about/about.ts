import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ButtonComponent } from '../../shared/ui/button/button';
import { IconComponent } from '../../shared/ui/icon/icon';
import { ImgFallbackDirective } from '../../shared/ui/img-fallback/img-fallback.directive';

interface Stat {
  icon: string;
  text: string;
}

@Component({
  selector: 'app-about',
  imports: [ButtonComponent, IconComponent, ImgFallbackDirective],
  template: `
    <div class="page">
      <div class="container">
        <div class="banner">
          <!-- TODO(IMAGES): заменить на реальное фото из API -->
          <img src="https://picsum.photos/seed/dp-about/1600/700" alt="Собака и кошка вместе" appImgFallback />
          <div class="banner__overlay"></div>
          <h1 class="banner__title">О нас</h1>
        </div>

        <div class="prose">
          <p>
            «Добрые лапки» — приют для животных, который с 2022 года помогает кошкам, собакам и
            другим питомцам найти любящих хозяев. Мы забираем животных с улицы, лечим, социализируем
            и подбираем им семьи.
          </p>
          <p>
            Вместе с волонтёрами мы заботимся о каждом подопечном, ведём учёт здоровья и прививок, а
            также сопровождаем новые семьи после усыновления. Каждое животное заслуживает дома — и
            мы верим, что вместе это возможно.
          </p>
        </div>

        <div class="stats">
          @for (stat of stats; track stat.text) {
            <div class="stat">
              <span class="stat__icon"><app-icon [name]="stat.icon" [size]="22" /></span>
              <p class="stat__text">{{ stat.text }}</p>
            </div>
          }
        </div>

        <div class="cta">
          <div>
            <h2 class="stats__title">Хотите подарить дом?</h2>
            <p class="catalog-link">Посмотрите животных, которые ждут свою семью прямо сейчас.</p>
          </div>
          <app-button routerLink="/animals" size="lg">
            Посмотреть животных
            <span class="btn__icon"><app-icon name="arrow-right" [size]="18" /></span>
          </app-button>
        </div>
      </div>
    </div>
  `,
  styles: `
    .banner {
      position: relative;
      border-radius: var(--radius-lg);
      overflow: hidden;
      min-height: 300px;
      display: flex;
      align-items: flex-end;
    }
    .banner img {
      position: absolute;
      inset: 0;
      width: 100%;
      height: 100%;
      object-fit: cover;
    }
    .banner__overlay {
      position: absolute;
      inset: 0;
      background: linear-gradient(180deg, rgba(30, 40, 35, 0.1), rgba(30, 40, 35, 0.7));
    }
    .banner__title {
      position: relative;
      z-index: 1;
      margin: var(--space-8) var(--space-10);
      color: #fff;
      font-size: 40px;
      font-weight: 800;
      letter-spacing: -0.02em;
    }
    .prose {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
      max-width: 760px;
      margin-top: var(--space-8);
      font-size: 16px;
      line-height: 1.7;
      color: var(--color-text);
    }
    .stats {
      display: grid;
      grid-template-columns: repeat(3, minmax(0, 1fr));
      gap: var(--space-6);
      margin-top: var(--space-10);
    }
    .stats > * {
      display: flex;
      align-items: center;
      gap: var(--space-4);
      padding: var(--space-6);
      background: var(--color-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
    }
    .stats app-icon {
      color: var(--color-primary);
    }
    .stat__text {
      font-weight: 600;
      font-size: 15px;
    }
    .prose + .stats {
      margin-top: var(--space-8);
    }
    @media (max-width: 760px) {
      .stats {
        grid-template-columns: 1fr;
      }
      .banner__title {
        margin: var(--space-6);
        font-size: 32px;
      }
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AboutPage {
  protected readonly stats: readonly Stat[] = [
    { icon: 'paw-print', text: 'Всего 500 животных нашли дом' },
    { icon: 'heart', text: 'Сотрудничаем с волонтёрами' },
    { icon: 'house', text: 'Помогаем с 2022 года' },
  ];
}
