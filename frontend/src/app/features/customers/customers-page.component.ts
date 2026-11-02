import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { EMPTY, Subject, catchError, debounceTime, distinctUntilChanged, finalize, map, switchMap } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { CustomerListItem, CustomerUpsert, WorkStatus } from '../../models/customer.models';
import { money } from '../../shared/format';
import { PageHeaderComponent } from '../../shared/page-header.component';

type CustomerFilter = 'all' | 'outstanding' | 'fullyPaid' | 'hasEstimates';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, PageHeaderComponent],
  template: `
    <app-page-header title="Customers" subtitle="Manage customer profiles, balances, estimates and payments.">
      <div actions class="flex flex-wrap gap-2">
        <details class="relative">
          <summary class="button button-secondary cursor-pointer list-none">Export <span aria-hidden="true">⌄</span></summary>
          <div class="export-menu">
            <button (click)="download(api.exportUrl('customers'), 'customers.xlsx')">Customers XLSX</button>
            <button (click)="download(api.exportUrl('outstanding'), 'outstanding-customers.xlsx')">Outstanding XLSX</button>
            <button (click)="download(api.exportUrl('fully-paid'), 'fully-paid-customers.xlsx')">Fully Paid XLSX</button>
          </div>
        </details>
        <button class="button button-primary" (click)="toggleForm()">{{ showForm ? 'Close' : '+ New Customer' }}</button>
      </div>
    </app-page-header>

    @if (toast) { <div class="notice notice-success mb-4" role="status">{{ toast }}</div> }
    <section class="content-card p-4 sm:p-5">
      <div class="customer-toolbar">
        <input class="field search-field" placeholder="Search name, phone, code or location" [formControl]="searchControl" (keyup.enter)="searchNow()" aria-label="Search customers" />
        <select class="field filter-field" [value]="filter" (change)="setFilter($any($event.target).value)" aria-label="Customer filter">
          <option value="all">All customers</option><option value="outstanding">Outstanding</option><option value="fullyPaid">Fully paid</option><option value="hasEstimates">Has estimates</option>
        </select>
        <button class="button button-secondary" (click)="searchNow()">Search</button>
        @if (searching) { <span class="inline-searching"><span class="spinner spinner-small"></span>Searching...</span> }
      </div>
      @if (listError) { <div class="notice notice-error mt-4"><span>{{ listError }}</span><button class="underline" (click)="load()">Retry</button></div> }
      @if (loadingList && !hasLoaded) {
        <div class="loading-state"><span class="spinner"></span>Loading customers...</div>
      } @else if (!customers.length && !listError) {
        <div class="empty-state"><strong>{{ searchControl.value || filter !== 'all' ? 'No customers match your search.' : 'No customers found' }}</strong><span>{{ searchControl.value || filter !== 'all' ? 'Try clearing the search or filters.' : 'Start by creating your first customer.' }}</span><button class="button button-primary" (click)="showForm = true">+ New Customer</button></div>
      } @else {
        <div class="customer-table-wrap">
          <table class="data-table">
            <thead><tr><th>Code</th><th>Customer</th><th>Phone</th><th>Location</th><th>Estimated</th><th>Received</th><th>Balance</th><th>Payment</th><th>Work</th><th>Actions</th></tr></thead>
            <tbody>
              @for (c of customers; track c.id) {
                <tr>
                  <td class="mono">{{ c.customerCode }}</td><td><strong>{{ c.name }}</strong></td><td>{{ c.phone }}</td><td>{{ c.location || '-' }}</td><td>{{ money(c.totalEstimateValue) }}</td><td>{{ money(c.totalReceived) }}</td><td class="font-semibold">{{ money(c.outstanding) }}</td><td><span class="status-pill" [class.status-paid]="c.outstanding <= 0 && c.estimateCount > 0">{{ c.estimateCount ? (c.outstanding > 0 ? 'Outstanding' : 'Fully paid') : 'No estimates' }}</span></td><td><span class="status-pill" [class]="'work-' + workStatusKey(c.latestWorkStatus)">{{ workStatusLabel(c.latestWorkStatus) }}</span></td>
                  <td><details class="relative"><summary class="action-menu-button" aria-label="Customer actions">⋮</summary><div class="export-menu action-menu"><a [routerLink]="['/customers', c.id]">View</a><button (click)="download(api.customerSummaryPdfUrl(c.id), c.customerCode + '-summary.pdf')">Summary PDF</button><button (click)="download(api.customerStatementPdfUrl(c.id), c.customerCode + '-statement.pdf')">Statement PDF</button><button (click)="download(api.customerStatementXlsxUrl(c.id), c.customerCode + '-statement.xlsx')">Statement XLSX</button></div></details></td>
                </tr>
                <tr class="mobile-customer-card"><td colspan="10"><strong>{{ c.name }}</strong><span>{{ c.customerCode }} · {{ c.phone }}</span><span>{{ c.location || 'No location' }} · {{ workStatusLabel(c.latestWorkStatus) }}</span><div><b>Estimated</b> {{ money(c.totalEstimateValue) }} <b>Received</b> {{ money(c.totalReceived) }} <b>Balance</b> {{ money(c.outstanding) }}</div><a class="text-brand-gold" [routerLink]="['/customers', c.id]">View customer</a></td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </section>

    @if (showForm) {
      <div class="modal-backdrop" (click)="closeForm()">
        <section class="modal-panel" (click)="$event.stopPropagation()" role="dialog" aria-modal="true" aria-labelledby="new-customer-title">
          <div class="flex items-start justify-between gap-4"><div><p class="eyebrow">Customer profile</p><h3 id="new-customer-title" class="text-xl font-semibold">New customer</h3></div><button class="icon-button" (click)="closeForm()" aria-label="Close">×</button></div>
          <form class="form-grid mt-5" [formGroup]="form" (ngSubmit)="save()">
            <label class="field-label field-wide">Name <span>*</span><input class="field" formControlName="name" autocomplete="name" /> @if (form.controls.name.touched && form.controls.name.invalid) { <small class="field-error">Name is required.</small> }</label>
            <label class="field-label">Phone <span>*</span><input class="field" formControlName="phone" autocomplete="tel" /> @if (form.controls.phone.touched && form.controls.phone.invalid) { <small class="field-error">Phone is required.</small> }</label>
            <label class="field-label">Alternate phone<input class="field" formControlName="alternatePhone" /></label>
            <label class="field-label">Email<input class="field" type="email" formControlName="email" /></label>
            <label class="field-label field-wide">Address<input class="field" formControlName="address" /></label>
            <label class="field-label">Location<input class="field" formControlName="location" /></label>
            <label class="field-label field-wide">Notes<textarea class="field" rows="3" formControlName="notes"></textarea></label>
            @if (formError) { <div class="notice notice-error field-wide">{{ formError }}</div> }
            <div class="modal-actions field-wide"><button type="button" class="button button-secondary" (click)="closeForm()">Cancel</button><button class="button button-primary" type="submit" [disabled]="form.invalid || saving">{{ saving ? 'Saving...' : 'Save customer' }}</button></div>
          </form>
        </section>
      </div>
    }
  `
})
export class CustomersPageComponent implements OnInit {
  protected readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private readonly searchRequests = new Subject<string>();
  customers: CustomerListItem[] = [];
  filter: CustomerFilter = 'all';
  showForm = false;
  loadingList = false;
  searching = false;
  hasLoaded = false;
  saving = false;
  listError = '';
  formError = '';
  toast = '';
  searchControl = this.fb.control('');
  form = this.fb.group({ name: ['', Validators.required], phone: ['', Validators.required], alternatePhone: [''], email: [''], address: [''], location: [''], notes: [''] });

  ngOnInit(): void {
    this.searchControl.valueChanges.pipe(map((value) => (value || '').trim()), debounceTime(350), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef)).subscribe((term) => this.searchRequests.next(term));
    this.searchRequests.pipe(switchMap((term) => this.searchRequest(term)), takeUntilDestroyed(this.destroyRef)).subscribe({ next: (res) => this.customers = res.items || [] });
    this.load();
  }

  load(): void {
    if (this.hasLoaded) { this.searchNow(); return; }
    this.loadingList = true;
    this.listError = '';
    const query: any = { search: this.searchControl.value || '', page: 1, pageSize: 50 };
    if (this.filter === 'outstanding') query.hasOutstanding = true;
    if (this.filter === 'fullyPaid') query.fullyPaid = true;
    if (this.filter === 'hasEstimates') query.hasEstimates = true;
    this.api.customers(query).pipe(finalize(() => this.loadingList = false)).subscribe({ next: (res) => { this.customers = res.items || []; this.hasLoaded = true; }, error: () => this.listError = 'Unable to load customers.' });
  }

  searchNow(): void { this.searchRequests.next((this.searchControl.value || '').trim()); }
  setFilter(value: CustomerFilter): void { this.filter = value; this.searchNow(); }
  toggleForm(): void { this.showForm ? this.closeForm() : (this.showForm = true); }
  closeForm(): void { if (!this.saving) { this.showForm = false; this.formError = ''; } }

  save(): void {
    if (this.form.invalid || this.saving) { this.form.markAllAsTouched(); return; }
    this.saving = true;
    this.formError = '';
    const value = this.form.getRawValue();
    const payload: CustomerUpsert = { name: (value.name || '').trim(), phone: (value.phone || '').trim(), alternatePhone: this.optional(value.alternatePhone), email: this.optional(value.email), address: this.optional(value.address), location: this.optional(value.location), notes: this.optional(value.notes), isActive: true };
    this.api.createCustomer(payload).pipe(finalize(() => this.saving = false)).subscribe({
      next: () => { this.form.reset(); this.showForm = false; this.toast = 'Customer saved successfully.'; this.load(); setTimeout(() => this.toast = '', 3500); },
      error: (error: HttpErrorResponse) => this.formError = this.apiError(error)
    });
  }

  download(url: string, filename: string): void { this.api.downloadFile(url).subscribe({ next: (blob) => { const link = document.createElement('a'); link.href = URL.createObjectURL(blob); link.download = filename; link.click(); URL.revokeObjectURL(link.href); }, error: (error: HttpErrorResponse) => this.toast = this.apiError(error) }); }
  private searchRequest(term: string) {
    this.searching = true;
    this.listError = '';
    const query: any = { search: term, page: 1, pageSize: 50 };
    if (this.filter === 'outstanding') query.hasOutstanding = true;
    if (this.filter === 'fullyPaid') query.fullyPaid = true;
    if (this.filter === 'hasEstimates') query.hasEstimates = true;
    return this.api.customers(query).pipe(finalize(() => this.searching = false), catchError(() => { this.listError = 'Unable to search customers.'; return EMPTY; }));
  }
  private optional(value: string | null): string | null { const trimmed = (value || '').trim(); return trimmed || null; }
  workStatusKey(status: WorkStatus | number | null | undefined): string { return typeof status === 'number' ? ({ 1: 'notstarted', 2: 'scheduled', 3: 'inprogress', 4: 'temporarilystopped', 5: 'permanentlystopped', 6: 'completed', 7: 'cancelled' } as Record<number, string>)[status] || 'notstarted' : (status || 'NotStarted').toLowerCase(); }
  workStatusLabel(status: WorkStatus | number | null | undefined): string { return ({ notstarted: 'Not started', scheduled: 'Scheduled', inprogress: 'In progress', temporarilystopped: 'Temp. stopped', permanentlystopped: 'Permanent stop', completed: 'Completed', cancelled: 'Cancelled' } as Record<string, string>)[this.workStatusKey(status)] || 'Not started'; }
  private apiError(error: HttpErrorResponse): string { const detail = error.error?.title || error.error?.detail || error.error?.message; if (detail) return detail; if (error.error?.errors) return Object.values(error.error.errors).flatMap((value) => Array.isArray(value) ? value : [String(value)]).join(' '); return error.status === 0 ? 'Unable to reach the API.' : `Request failed (${error.status}).`; }
  protected readonly money = money;
}
