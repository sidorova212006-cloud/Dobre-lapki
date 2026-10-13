import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ButtonComponent } from '../../shared/ui/button/button';
import { IconComponent } from '../../shared/ui/icon/icon';

interface Contact {
  icon: string;
  title: string;
  value: string;
}

@Component({
  selector: 'app-contacts',
  imports: [ButtonComponent, IconComponent],
  template: `
    <div class="page">
      <div class="container">
        <header class="head">
          <h1 class="head__title">Контакты</h1>
          <p class="head__text">
            Приезжайте знакомиться с питомцами или напишите нам — мы рады помочь и ответить на
            вопросы об усыновлении.
          </p>
        </header>

        <div class="contacts">
          @for (contact of contacts; track contact.title) {
            <div class="contact">
              <span class="contact__icon"><app-icon [name]="contact.icon" [size]="20" /></span>
              <div>
                <p class="contact__title">{{ contact.title }}</p>
                <p class="contact__value">{{ contact.value }}</p>
              </div>
            </div>
          }
        </div>

        <div class="cta">
          <div>
            <h2 class="cta__title">Хотите забрать питомца домой?</h2>
            <p class="cta__text">Начните с каталога — выберите того, кто ждёт именно вас.</p>
          </div>
          <app-button routerLink="/animals">Посмотреть животных</app-button>
        </div>
      </div>
    </div>
  `,
  styles: `
    .head__title {
      font-size: 32px;
      font-weight: 800;
      letter-spacing: -0.02em;
    }
    .head__text {
      margin-top: var(--space-2);
      max-width: 60ch;
      color: var(--color-text-muted);
      font-size: 15px;
    }
    .contacts {
      display: grid;
      grid-template-columns: repeat(3, minmax(0, 1fr));
      gap: var(--space-6);
      margin-top: var(--space-8);
    }
    .contact {
      display: flex;
      align-items: center;
      gap: var(--space-4);
      padding: var(--space-6);
      background: var(--color-surface);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
    }
    .contact__icon {
      display: grid;
      place-items: center;
      flex: none;
      width: 44px;
      height: 44px;
      border-radius: var(--radius-sm);
      background: var(--color-primary-soft);
      color: var(--color-primary);
    }
    .contact__title {
      color: var(--color-text-muted);
      font-size: 13px;
    }
    .contact__value {
      font-weight: 600;
      font-size: 15px;
    }
    .cta {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: var(--space-6);
      flex-wrap: wrap;
      margin-top: var(--space-10);
      padding: var(--space-8);
      background: var(--color-primary-soft);
      border-radius: var(--radius-lg);
    }
    .cta__title {
      font-size: 22px;
      font-weight: 700;
    }
    .cta__text {
      margin-top: 4px;
      color: var(--color-text);
      font-size: 14px;
    }
    @media (max-width: 760px) {
      .contacts {
        grid-template-columns: 1fr;
      }
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContactsPage {
  protected readonly contacts: readonly Contact[] = [
    { icon: 'map-pin', title: 'Адрес', value: 'г. Москва, ул. Добрая, 12' },
    { icon: 'phone', title: 'Телефон', value: '+7 (900) 123-45-67' },
    { icon: 'mail', title: 'Email', value: 'hello@dobrye-lapki.ru' },
  ];
}
