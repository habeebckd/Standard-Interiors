import { Component, OnInit, inject } from '@angular/core';
import { forkJoin } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { money, indianDate } from '../../shared/format';

@Component({
  standalone: true,
  imports: [PageHeaderComponent],
  template: `
    <app-page-header title="Reports" subtitle="Export and review all customer, estimate and payment summaries.">
      <div actions class="flex flex-wrap gap-2">
        <a [href]="api.exportUrl('outstanding')" class="rounded-full border border-black/10 px-3 py-2 text-xs">Outstanding XLSX</a>
        <a [href]="api.exportUrl('fully-paid')" class="rounded-full border border-black/10 px-3 py-2 text-xs">Fully Paid XLSX</a>
        <a [href]="api.exportUrl('monthly-collections')" class="rounded-full bg-brand-gold px-3 py-2 text-xs font-semibold">Collections XLSX</a>
      </div>
    </app-page-header>

    @if (loading) { <div class="loading-state"><span class="spinner"></span>Loading reports...</div> } @else if (error) { <div class="notice notice-error"><span>{{ error }}</span><button class="underline" (click)="load()">Retry</button></div> } @else { <div class="grid gap-6 xl:grid-cols-2">
      <section class="rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
        <h3 class="text-lg font-semibold">Outstanding Customers</h3>
        <div class="mt-3 overflow-x-auto"><table class="report-table text-sm"><thead class="text-xs uppercase text-black/45"><tr><th class="py-2">Customer</th><th>Balance</th></tr></thead><tbody>@for (row of outstanding; track row.customerCode) {<tr><td class="py-2">{{ row.customerName }}</td><td>{{ money(row.outstanding) }}</td></tr>}</tbody></table></div>
      </section>

      <section class="rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
        <h3 class="text-lg font-semibold">Monthly Collections</h3>
        <div class="mt-3 overflow-x-auto"><table class="report-table text-sm"><thead class="text-xs uppercase text-black/45"><tr><th class="py-2">Month</th><th>Received</th></tr></thead><tbody>@for (row of monthly; track row.month) {<tr><td class="py-2">{{ row.month }}</td><td>{{ money(row.totalReceived) }}</td></tr>}</tbody></table></div>
      </section>

      <section class="rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
        <h3 class="text-lg font-semibold">All Estimates</h3>
        <div class="mt-3 overflow-x-auto"><table class="report-table text-sm"><thead class="text-xs uppercase text-black/45"><tr><th class="py-2">Estimate</th><th>Customer</th><th>Date</th><th>Total</th></tr></thead><tbody>@for (row of estimates; track row.estimateNumber) {<tr><td class="py-2">{{ row.estimateNumber }}</td><td>{{ row.customerName }}</td><td>{{ indianDate(row.estimateDate) }}</td><td>{{ money(row.totalAmount) }}</td></tr>}</tbody></table></div>
      </section>

      <section class="rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
        <h3 class="text-lg font-semibold">All Payments</h3>
        <div class="mt-3 overflow-x-auto"><table class="report-table text-sm"><thead class="text-xs uppercase text-black/45"><tr><th class="py-2">Payment</th><th>Customer</th><th>Date</th><th>Amount</th></tr></thead><tbody>@for (row of payments; track row.paymentNumber) {<tr><td class="py-2">{{ row.paymentNumber }}</td><td>{{ row.customerName }}</td><td>{{ indianDate(row.paymentDate) }}</td><td>{{ money(row.amount) }}</td></tr>}</tbody></table></div>
      </section>
    </div> }
  `
})
export class ReportsPageComponent implements OnInit {
  protected readonly api = inject(ApiService);
  outstanding: any[] = [];
  monthly: any[] = [];
  estimates: any[] = [];
  payments: any[] = [];
  loading = true;
  error = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void { this.loading = true; this.error = ''; forkJoin({ outstanding: this.api.reportsOutstanding(), monthly: this.api.reportsMonthlyCollections(), estimates: this.api.reportsEstimates(), payments: this.api.reportsPayments() }).subscribe({ next: (res) => { this.outstanding = res.outstanding; this.monthly = res.monthly; this.estimates = res.estimates; this.payments = res.payments; this.loading = false; }, error: () => { this.error = 'Unable to load reports.'; this.loading = false; } }); }

  protected readonly money = money;
  protected readonly indianDate = indianDate;
}
