import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { FavoritesService } from '../../core/services/favorites.service';
import { SessionService } from '../../core/services/session.service';
import { IconComponent } from '../../shared/ui/icon/icon';
import { ImgFallbackDirective } from '../../shared/ui/img-fallback/img-fallback.directive';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, IconComponent, ImgFallbackDirective],
  templateUrl: './header.html',
  styleUrl: './header.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent {
  private readonly favorites = inject(FavoritesService);
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);

  protected readonly user = this.session.user;
  protected readonly favoritesCount = computed(() => this.favorites.count());
  protected readonly menuOpen = signal(false);
  protected readonly searchOpen = signal(false);
  protected readonly query = signal('');

  constructor() {
    this.favorites.ensureLoaded();
  }

  protected toggleSearch(): void {
    this.searchOpen.update((open) => !open);
  }

  protected onQuery(event: Event): void {
    this.query.set((event.target as HTMLInputElement).value);
  }

  protected submitSearch(): void {
    const value = this.query().trim();
    this.searchOpen.set(false);
    void this.router.navigate(['/animals'], {
      queryParams: value ? { q: value } : {},
    });
  }

  protected toggleMenu(): void {
    this.menuOpen.update((open) => !open);
  }

  protected closeMenu(): void {
    this.menuOpen.set(false);
  }
}
