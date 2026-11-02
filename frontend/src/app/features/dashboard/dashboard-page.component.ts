import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { DashboardModel } from '../../models/dashboard.models';
import { money, indianDate } from '../../shared/format';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { StatCardComponent } from '../../shared/stat-card.component';
import { StatusBadgeComponent, StatusTone } from '../../shared/status-badge.component';

@Component({
  standalone: true,
  imports: [RouterLink, PageHeaderComponent, StatCardComponent, StatusBadgeComponent],
  template: `
    <app-page-header title="Dashboard" subtitle="Live business overview from SQL Server and the API."></app-page-header>

    @if (loading) {
      <div class="rounded-2xl border border-black/10 bg-white p-8 text-sm text-black/60">Loading dashboard...</div>
    } @else if (error) {
      <div class="rounded-2xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">{{ error }}</div>
    } @else if (data) {
      <div class="grid gap-4 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-6">
        <app-stat-card label="Total Customers" [value]="data.totalCustomers.toString()"></app-stat-card>
        <app-stat-card label="Total Estimates" [value]="data.totalEstimates.toString()"></app-stat-card>
        <app-stat-card label="Estimated Value" [value]="money(data.totalEstimatedValue)"></app-stat-card>
        <app-stat-card label="Amount Received" [value]="money(data.totalAmountReceived)"></app-stat-card>
        <app-stat-card label="Outstanding" [value]="money(data.totalOutstandingBalance)"></app-stat-card>
        <app-stat-card label="This Month" [value]="money(data.thisMonthReceived)"></app-stat-card>
      </div>
      @if (data.workStatusSummary?.length) { <section class="content-card mt-5 p-5"><div class="flex items-center justify-between"><div><p class="eyebrow">Project status</p><h3 class="text-lg font-semibold text-brand-charcoal">Work execution overview</h3></div><a routerLink="/customers" class="text-sm text-brand-gold">View customers</a></div><div class="work-summary-grid mt-4">@for (item of data.workStatusSummary; track item.status) { <div><span class="status-pill" [class]="'work-' + statusKey(item.status)">{{ statusLabel(item.status) }}</span><strong>{{ item.count }}</strong></div> }</div></section> }

      <div class="mt-6 grid gap-6 xl:grid-cols-2">
        <section class="rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
          <h3 class="text-lg font-semibold text-brand-charcoal">Recent Estimates</h3>
          <div class="mt-4 overflow-x-auto">
            <table class="dashboard-table dashboard-estimates text-left text-sm">
              <thead class="text-xs uppercase tracking-wide text-black/45">
                <tr><th class="py-2">No</th><th>Customer</th><th>Date</th><th>Amount</th><th>Status</th><th>Balance</th></tr>
              </thead>
              <tbody>
                @for (item of data.recentEstimates; track item.estimateNumber) {
                  <tr class="border-t border-black/5">
                    <td class="py-2 font-medium">{{ item.estimateNumber }}</td>
                    <td>{{ item.customer }}</td>
                    <td>{{ indianDate(item.date) }}</td>
                    <td>{{ money(item.amount) }}</td>
                    <td><app-status-badge [label]="item.status" [tone]="estimateTone(item.status)"></app-status-badge></td>
                    <td>{{ money(item.balance) }}</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </section>

        <section class="rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
          <h3 class="text-lg font-semibold text-brand-charcoal">Recent Payments</h3>
          <div class="mt-4 overflow-x-auto">
            <table class="dashboard-table dashboard-payments text-left text-sm">
              <thead class="text-xs uppercase tracking-wide text-black/45">
                <tr><th class="py-2">Date</th><th>Customer</th><th>Amount</th><th>Method</th><th>Reference</th></tr>
              </thead>
              <tbody>
                @for (item of data.recentPayments; track item.date + item.customer) {
                  <tr class="border-t border-black/5">
                    <td class="py-2">{{ indianDate(item.date) }}</td>
                    <td>{{ item.customer }}</td>
                    <td>{{ money(item.amount) }}</td>
                    <td>{{ item.method }}</td>
                    <td>{{ item.reference || '-' }}</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </section>
      </div>

      <section class="mt-6 rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
        <h3 class="text-lg font-semibold text-brand-charcoal">Customers With Outstanding Balance</h3>
        <div class="mt-4 overflow-x-auto">
          <table class="dashboard-table text-left text-sm">
            <thead class="text-xs uppercase tracking-wide text-black/45">
              <tr><th class="py-2">Customer</th><th>Estimate Value</th><th>Received</th><th>Balance</th></tr>
            </thead>
            <tbody>
              @for (item of data.outstandingCustomers; track item.customer) {
                <tr class="border-t border-black/5">
                  <td class="py-2 font-medium">{{ item.customer }}</td>
                  <td>{{ money(item.estimateValue) }}</td>
                  <td>{{ money(item.received) }}</td>
                  <td>{{ money(item.balance) }}</td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      </section>
    }
  `
})
export class DashboardPageComponent implements OnInit {
  private readonly api = inject(ApiService);
  data: DashboardModel | null = null;
  loading = true;
  error = '';

  ngOnInit(): void {
    this.api.dashboard().subscribe({
      next: (data) => { this.data = data; this.loading = false; },
      error: () => { this.error = 'Unable to load dashboard data.'; this.loading = false; }
    });
  }

  protected readonly money = money;
  protected readonly indianDate = indianDate;
  statusKey(status: string | number): string { return typeof status === 'number' ? ({ 1: 'notstarted', 2: 'scheduled', 3: 'inprogress', 4: 'temporarilystopped', 5: 'permanentlystopped', 6: 'completed', 7: 'cancelled' } as Record<number, string>)[status] || 'notstarted' : status.toLowerCase(); }
  statusLabel(status: string | number): string { return ({ notstarted: 'Not Started', scheduled: 'Scheduled', inprogress: 'In Progress', temporarilystopped: 'Temporarily Stopped', permanentlystopped: 'Permanently Stopped', completed: 'Completed', cancelled: 'Cancelled' } as Record<string, string>)[this.statusKey(status)] || 'Not Started'; }
  estimateTone(status: string): StatusTone { return status === 'Completed' || status === 'Approved' ? 'green' : status === 'Cancelled' || status === 'Rejected' ? 'red' : status === 'Sent' ? 'blue' : 'neutral'; }
}
