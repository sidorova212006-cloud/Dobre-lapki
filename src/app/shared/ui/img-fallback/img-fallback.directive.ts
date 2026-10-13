import { Directive, ElementRef, inject } from '@angular/core';

// TODO(IMAGES): заменить picsum-заглушки на реальные фото из API.
const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="800" height="800" viewBox="0 0 800 800">
  <rect width="800" height="800" fill="#E4EDE9"/>
  <g fill="#2F5D50" opacity="0.5">
    <ellipse cx="400" cy="520" rx="130" ry="105"/>
    <circle cx="262" cy="392" r="62"/>
    <circle cx="368" cy="322" r="64"/>
    <circle cx="474" cy="330" r="62"/>
    <circle cx="566" cy="398" r="58"/>
  </g>
</svg>`;

/** Локальная заглушка на случай, если фото недоступно. */
export const PHOTO_PLACEHOLDER = `data:image/svg+xml;charset=utf-8,${encodeURIComponent(svg)}`;

/**
 * Подставляет локальную заглушку, если внешнее изображение не загрузилось,
 * чтобы интерфейс не показывал «сломанные» картинки.
 *
 * <img [src]="animal.photos[0]" appImgFallback />
 */
@Directive({
  selector: 'img[appImgFallback]',
  host: { '(error)': 'handleError()' },
})
export class ImgFallbackDirective {
  private readonly element = inject<ElementRef<HTMLImageElement>>(ElementRef);

  protected handleError(): void {
    const image = this.element.nativeElement;
    if (image.src === PHOTO_PLACEHOLDER) {
      return;
    }
    image.removeAttribute('srcset');
    image.src = PHOTO_PLACEHOLDER;
  }
}
