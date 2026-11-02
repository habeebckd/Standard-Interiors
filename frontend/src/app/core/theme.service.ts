import { DOCUMENT } from '@angular/common';
import { inject, Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  private readonly storageKey = 'standard-interiors.theme';
  readonly dark = signal(this.readTheme());

  constructor() { this.apply(this.dark()); }

  toggle(): void {
    const value = !this.dark();
    this.dark.set(value);
    localStorage.setItem(this.storageKey, value ? 'dark' : 'light');
    this.apply(value);
  }

  private readTheme(): boolean {
    return localStorage.getItem(this.storageKey) === 'dark';
  }

  private apply(dark: boolean): void {
    this.document.body.classList.toggle('dark-mode', dark);
  }
}
