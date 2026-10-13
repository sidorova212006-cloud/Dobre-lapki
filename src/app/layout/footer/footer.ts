import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { IconComponent } from '../../shared/ui/icon/icon';

@Component({
  selector: 'app-footer',
  imports: [RouterLink, IconComponent],
  template: `
    <footer class="footer">
      <div class="container footer__inner">
        <div class="footer__brand">
          <span class="brand__mark"><app-icon name="paw-print" [size]="18" /></span>
          <div>
            <p class="footer__name">Добрые лапки</p>
            <p class="footer__tagline">Приют для животных, где каждого ждут дом и забота.</p>
          </div>
        </div>

        <nav class="footer__nav" aria-label="Навигация в подвале">
          <a routerLink="/animals">Животные</a>
          <a routerLink="/about">О нас</a>
          <a routerLink="/favorites">Избранное</a>
          <a routerLink="/profile">Профиль</a>
        </nav>

        <div class="footer__contacts">
          <span><app-icon name="map-pin" [size]="16" /> г. Москва, ул. Добрая, 12</span>
          <span><app-icon name="phone" [size]="16" /> +7 (900) 123-45-67</span>
          <span><app-icon name="mail" [size]="16" /> hello@dobrye-lapki.ru</span>
        </div>
      </div>
      <div class="container footer__bottom">
        <span>© {{ year }} Приют «Добрые лапки». Сделано с любовью.</span>
      </div>
    </footer>
  `,
  styles: `
    .footer {
      margin-top: var(--space-16);
      background: var(--color-surface);
      border-top: 1px solid var(--color-border);
      padding-block: var(--space-10) var(--space-6);
    }
    .footer__inner {
      display: flex;
      flex-wrap: wrap;
      gap: var(--space-8);
      justify-content: space-between;
    }
    .footer__brand {
      display: flex;
      gap: var(--space-3);
      max-width: 320px;
    }
    .brand__mark {
      display: grid;
      place-items: center;
      flex: none;
      width: 34px;
      height: 34px;
      border-radius: 10px;
      background: var(--color-primary);
      color: #fff;
    }
    .footer__name {
      font-weight: 800;
      font-size: 16px;
    }
    .footer__tagline {
      color: var(--color-text-muted);
      font-size: 14px;
    }
    .footer__nav {
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
      font-size: 14px;
      font-weight: 500;
      color: var(--color-text-muted);
    }
    .footer__nav a:hover {
      color: var(--color-primary);
    }
    .footer__contacts {
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
      font-size: 14px;
      color: var(--color-text-muted);
    }
    .footer__contacts span {
      display: inline-flex;
      align-items: center;
      gap: var(--space-2);
    }
    .footer__bottom {
      margin-top: var(--space-8);
      padding-top: var(--space-5);
      border-top: 1px solid var(--color-border);
      color: var(--color-text-muted);
      font-size: 13px;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FooterComponent {
  protected readonly year = new Date().getFullYear();
}
