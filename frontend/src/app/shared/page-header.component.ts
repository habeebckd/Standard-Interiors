import { Component, input } from '@angular/core';

@Component({
  selector: 'app-page-header',
  standalone: true,
  template: `
    <div class="page-header mb-6 flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
      <div>
        <h2 class="text-2xl font-semibold text-brand-charcoal">{{ title() }}</h2>
        @if (subtitle()) {
          <p class="mt-1 text-sm text-black/60">{{ subtitle() }}</p>
        }
      </div>
      <ng-content select="[actions]"></ng-content>
    </div>
  `
})
export class PageHeaderComponent {
  readonly title = input.required<string>();
  readonly subtitle = input<string>('');
}
