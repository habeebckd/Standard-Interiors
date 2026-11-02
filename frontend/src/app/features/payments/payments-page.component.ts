import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { CustomerListItem } from '../../models/customer.models';
import { PaymentListItem, PaymentUpsert } from '../../models/payment.models';
import { PaymentMethod } from '../../models/service.models';
import { money, indianDate } from '../../shared/format';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { StatusBadgeComponent } from '../../shared/status-badge.component';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent, StatusBadgeComponent],
  template: `
    <app-page-header title="Payments" subtitle="Record receipts, void mistakes safely and print payment sheets.">
      <div actions><a [href]="exportUrl" class="rounded-full border border-black/10 px-4 py-2 text-sm">Export XLSX</a></div>
    </app-page-header>

    @if (loading) { <div class="loading-state"><span class="spinner"></span>Loading payments...</div> } @else if (error) { <div class="notice notice-error"><span>{{ error }}</span><button class="underline" (click)="load()">Retry</button></div> } @else { <div class="grid gap-6 xl:grid-cols-[1.2fr_0.8fr]">
      <section class="rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
        <div class="overflow-x-auto">
          <table class="data-table payments-table text-left text-sm">
            <thead class="text-xs uppercase tracking-wide text-black/45">
              <tr><th class="py-2">Payment No</th><th>Date</th><th>Customer</th><th>Estimate</th><th>Amount</th><th>Method</th><th>Reference</th><th>Status</th></tr>
            </thead>
            <tbody>
              @for (p of payments; track p.id) {
                <tr class="border-t border-black/5">
                  <td class="py-2 font-medium">{{ p.paymentNumber }}</td>
                  <td>{{ indianDate(p.paymentDate) }}</td>
                  <td>{{ p.customerName }}</td>
                  <td>{{ p.estimateNumber || '-' }}</td>
                  <td>{{ money(p.amount) }}</td>
                  <td>{{ p.paymentMethodName }}</td>
                  <td>{{ p.transactionReference || '-' }}</td>
                  <td><app-status-badge [label]="p.status" [tone]="p.status === 'Posted' ? 'green' : 'red'"></app-status-badge></td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      </section>

      <section class="rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
        <h3 class="text-lg font-semibold">New Payment</h3>
        <form class="mt-4 space-y-3" [formGroup]="form" (ngSubmit)="save()">
          <select class="w-full rounded-xl border border-black/10 px-4 py-2" formControlName="customerId">
            <option value="">Select customer</option>
            @for (c of customers; track c.id) { <option [value]="c.id">{{ c.customerCode }} - {{ c.name }}</option> }
          </select>
          <select class="w-full rounded-xl border border-black/10 px-4 py-2" formControlName="estimateId">
            <option value="">Optional estimate</option>
            @for (e of estimates; track e.id) { <option [value]="e.id">{{ e.estimateNumber }} - {{ e.customerName }}</option> }
          </select>
          <input class="w-full rounded-xl border border-black/10 px-4 py-2" type="date" formControlName="paymentDate" />
          <input class="w-full rounded-xl border border-black/10 px-4 py-2" placeholder="Amount" formControlName="amount" />
          <select class="w-full rounded-xl border border-black/10 px-4 py-2" formControlName="paymentMethodId">
            <option value="">Select payment method</option>
            @for (m of paymentMethods; track m.id) { <option [value]="m.id">{{ m.name }}</option> }
          </select>
          <input class="w-full rounded-xl border border-black/10 px-4 py-2" placeholder="Transaction Reference" formControlName="transactionReference" />
          <textarea class="w-full rounded-xl border border-black/10 px-4 py-2" rows="3" placeholder="Note" formControlName="notes"></textarea>
          <button class="rounded-full bg-brand-gold px-4 py-2 text-sm font-semibold text-black" type="submit" [disabled]="form.invalid">Save Payment</button>
        </form>
      </section>
    </div> }
  `
})
export class PaymentsPageComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);

  customers: CustomerListItem[] = [];
  estimates: any[] = [];
  paymentMethods: PaymentMethod[] = [];
  payments: PaymentListItem[] = [];
  loading = true;
  error = '';
  form = this.fb.group({
    customerId: ['', Validators.required],
    estimateId: [''],
    paymentDate: [new Date().toISOString().slice(0, 10), Validators.required],
    amount: ['', Validators.required],
    paymentMethodId: ['', Validators.required],
    transactionReference: [''],
    notes: ['']
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true; this.error = '';
    forkJoin({ customers: this.api.customers({ pageSize: 200 }), estimates: this.api.estimates({ pageSize: 200 }), methods: this.api.paymentMethods(), payments: this.api.payments({ pageSize: 100 }) }).subscribe({ next: (res) => { this.customers = res.customers.items || []; this.estimates = res.estimates.items || []; this.paymentMethods = res.methods || []; this.payments = res.payments.items || []; this.loading = false; }, error: () => { this.error = 'Unable to load payments.'; this.loading = false; } });
  }

  get exportUrl(): string { return this.api.exportUrl('payments'); }

  save(): void {
    if (this.form.invalid) return;
    const raw = this.form.getRawValue() as any;
    const payload: PaymentUpsert = {
      customerId: Number(raw.customerId),
      estimateId: raw.estimateId ? Number(raw.estimateId) : null,
      paymentDate: raw.paymentDate,
      amount: Number(raw.amount),
      paymentMethodId: Number(raw.paymentMethodId),
      transactionReference: raw.transactionReference,
      notes: raw.notes,
      allowOverpayment: false
    };
    this.api.createPayment(payload).subscribe(() => {
      this.form.patchValue({ transactionReference: '', notes: '' });
      this.load();
    });
  }

  protected readonly money = money;
  protected readonly indianDate = indianDate;
}
