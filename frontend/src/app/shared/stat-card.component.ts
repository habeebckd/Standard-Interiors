import { Component, input } from '@angular/core';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  template: `
    <div class="rounded-2xl border border-black/10 bg-white p-4 shadow-sm">
      <div class="text-xs uppercase tracking-[0.28em] text-black/45">{{ label() }}</div>
      <div class="mt-2 text-2xl font-semibold text-brand-charcoal">{{ value() }}</div>
      @if (hint()) {
        <div class="mt-1 text-xs text-black/45">{{ hint() }}</div>
      }
    </div>
  `
})
export class StatCardComponent {
  readonly label = input.required<string>();
  readonly value = input.required<string>();
  readonly hint = input<string>('');
}
