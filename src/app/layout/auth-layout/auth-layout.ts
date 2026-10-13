import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { IconComponent } from '../../shared/ui/icon/icon';
import { ImgFallbackDirective } from '../../shared/ui/img-fallback/img-fallback.directive';

/** Двухпанельный экран авторизации: фото слева, форма справа. */
@Component({
  selector: 'app-auth-layout',
  imports: [IconComponent, ImgFallbackDirective],
  template: `
    <div class="auth">
      <aside class="auth__aside">
        <!-- TODO(IMAGES): заменить на реальное фото из API -->
        <img class="auth__photo" src="https://picsum.photos/seed/dp-auth/1200/1400" alt="" appImgFallback />
        <div class="auth__overlay"></div>
        <div class="auth__content">
          <span class="auth__icon"><app-icon name="heart" [size]="26" /></span>
          <h2 class="auth__slogan">{{ slogan() }}</h2>
          <p class="auth__text">
            Тысячи животных ищут семью. Помоги одному из них обрести дом — начни с заявки.
          </p>
        </div>
      </aside>

      <section class="auth__panel">
        <div class="auth__card">
          <ng-content />
        </div>
      </section>
    </div>
  `,
  styles: `
    .auth {
      display: grid;
      grid-template-columns: 55% 45%;
      min-height: 100vh;
    }

    .auth__aside {
      position: relative;
      overflow: hidden;
      background: var(--color-primary);
    }

    .auth__photo {
      position: absolute;
      inset: 0;
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .auth__overlay {
      position: absolute;
      inset: 0;
      background: linear-gradient(
        180deg,
        rgba(30, 40, 35, 0.35) 0%,
        rgba(30, 40, 35, 0.72) 100%
      );
    }

    .auth__content {
      position: relative;
      z-index: 1;
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
      justify-content: flex-end;
      height: 100%;
      padding: var(--space-12);
      color: #fff;
    }

    .auth__icon {
      display: grid;
      place-items: center;
      width: 52px;
      height: 52px;
      border-radius: var(--radius-pill);
      border: 1.5px solid rgba(255, 255, 255, 0.6);
    }

    .auth__slogan {
      font-size: 34px;
      font-weight: 800;
      line-height: 1.15;
      letter-spacing: -0.02em;
      max-width: 16ch;
    }

    .auth__text {
      max-width: 42ch;
      color: rgba(255, 255, 255, 0.82);
      font-size: 15px;
    }

    .auth__panel {
      display: grid;
      place-items: center;
      padding: var(--space-8);
      background: var(--color-bg);
    }

    .auth__card {
      width: 100%;
      max-width: 400px;
    }

    @media (max-width: 860px) {
      .auth {
        grid-template-columns: 1fr;
      }
      .auth__aside {
        min-height: 240px;
      }
      .auth__content {
        padding: var(--space-8);
      }
      .auth__slogan {
        font-size: 26px;
      }
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuthLayoutComponent {
  readonly slogan = input<string>('Дай дом тому, кто ждёт');
}
