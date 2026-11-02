import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { EstimateListItem } from '../../models/estimate.models';
import { money, indianDate } from '../../shared/format';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { StatusBadgeComponent, StatusTone } from '../../shared/status-badge.component';

@Component({
  standalone: true,
  imports: [RouterLink, PageHeaderComponent, StatusBadgeComponent],
  template: `
    <app-page-header title="Estimates" subtitle="Create, revise, duplicate and print customer estimates.">
      <div actions>
        <div class="flex gap-2">
          <a [href]="exportUrl" class="rounded-full border border-black/10 px-4 py-2 text-sm">Export XLSX</a>
          <a routerLink="/estimates/new" class="rounded-full bg-brand-gold px-4 py-2 text-sm font-semibold text-black">New Estimate</a>
        </div>
      </div>
    </app-page-header>

    @if (loading) { <div class="loading-state"><span class="spinner"></span>Loading estimates...</div> } @else if (error) { <div class="notice notice-error"><span>{{ error }}</span><button class="underline" (click)="load()">Retry</button></div> } @else { <section class="content-card p-5">
      <div class="overflow-x-auto">
        <table class="data-table estimates-table text-left text-sm">
          <thead class="text-xs uppercase tracking-wide text-black/45">
            <tr><th class="py-2">Estimate No</th><th>Customer</th><th>Date</th><th>Sections</th><th>Total</th><th>Received</th><th>Balance</th><th>Status</th><th>Actions</th></tr>
          </thead>
          <tbody>
            @for (e of estimates; track e.id) {
              <tr class="border-t border-black/5">
                <td class="py-2 font-medium">{{ e.estimateNumber }}</td>
                <td>{{ e.customerName }}</td>
                <td>{{ indianDate(e.estimateDate) }}</td>
                <td>{{ e.sectionCount }}</td>
                <td>{{ money(e.totalAmount) }}</td>
                <td>{{ money(e.receivedAmount) }}</td>
                <td>{{ money(e.balance) }}</td>
                <td><app-status-badge [label]="e.status" [tone]="estimateTone(e.status)"></app-status-badge></td>
                <td><a [routerLink]="['/estimates', e.id]" class="text-brand-gold">Edit</a></td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </section> }
  `
})
export class EstimatesPageComponent implements OnInit {
  private readonly api = inject(ApiService);
  estimates: EstimateListItem[] = [];
  loading = true;
  error = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void { this.loading = true; this.error = ''; this.api.estimates({ pageSize: 100 }).subscribe({ next: (res) => { this.estimates = res.items || []; this.loading = false; }, error: () => { this.error = 'Unable to load estimates.'; this.loading = false; } }); }

  get exportUrl(): string { return this.api.exportUrl('estimates'); }
  estimateTone(status: string): StatusTone { return status === 'Completed' || status === 'Approved' ? 'green' : status === 'Cancelled' || status === 'Rejected' ? 'red' : status === 'Sent' ? 'blue' : 'neutral'; }

  protected readonly money = money;
  protected readonly indianDate = indianDate;
}
