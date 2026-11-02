import { Component, input } from '@angular/core';

export type StatusTone = 'neutral' | 'gold' | 'blue' | 'orange' | 'green' | 'red';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  template: `<span class="status-badge" [class]="'status-badge-' + tone()">{{ label() }}</span>`
})
export class StatusBadgeComponent {
  readonly label = input.required<string>();
  readonly tone = input<StatusTone>('neutral');
}
