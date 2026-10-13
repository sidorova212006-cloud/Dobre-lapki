import { ChangeDetectionStrategy, Component, forwardRef, input, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { IconComponent } from '../icon/icon';

export type InputType = 'text' | 'email' | 'password' | 'tel' | 'number';

@Component({
  selector: 'app-input',
  imports: [IconComponent],
  providers: [
    { provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => InputComponent), multi: true },
  ],
  template: `
    <label class="field">
      @if (label()) {
        <span class="field__label">{{ label() }}</span>
      }
      <span class="field__control">
        @if (icon(); as iconName) {
          <span class="field__icon"><app-icon [name]="iconName" [size]="18" /></span>
        }
        <input
          class="input"
          [class.input--with-icon]="icon() !== null"
          [class.input--invalid]="error() !== null"
          [type]="type()"
          [placeholder]="placeholder()"
          [attr.autocomplete]="autocomplete()"
          [attr.inputmode]="inputMode()"
          [attr.aria-invalid]="error() !== null"
          [value]="value()"
          [disabled]="isDisabled()"
          (input)="handleInput($event)"
          (blur)="onTouched()"
        />
      </span>
      @if (error()) {
        <span class="field__error">{{ error() }}</span>
      } @else if (hint()) {
        <span class="field__hint">{{ hint() }}</span>
      }
    </label>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InputComponent implements ControlValueAccessor {
  readonly label = input<string>('');
  readonly type = input<InputType>('text');
  readonly placeholder = input<string>('');
  readonly icon = input<string | null>(null);
  readonly error = input<string | null>(null);
  readonly hint = input<string>('');
  readonly autocomplete = input<string | null>(null);
  readonly inputMode = input<string | null>(null);

  protected readonly value = signal('');
  protected readonly isDisabled = signal(false);

  private onChange: (value: string) => void = () => undefined;
  protected onTouched: () => void = () => undefined;

  writeValue(value: string | null): void {
    this.value.set(value ?? '');
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled.set(isDisabled);
  }

  protected handleInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.value.set(target.value);
    this.onChange(target.value);
  }
}
