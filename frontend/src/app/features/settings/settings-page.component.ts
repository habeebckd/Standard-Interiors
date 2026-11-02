import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../core/api.service';
import { CompanySettings } from '../../models/service.models';
import { PageHeaderComponent } from '../../shared/page-header.component';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent],
  template: `
    <app-page-header title="Settings" subtitle="Company details, prefixes and default estimate note."></app-page-header>
    @if (loading) { <div class="loading-state"><span class="spinner"></span>Loading settings...</div> } @else if (error) { <div class="notice notice-error"><span>{{ error }}</span><button class="underline" (click)="load()">Retry</button></div> } @else { <section class="content-card max-w-3xl p-5">
      <form class="grid gap-3 md:grid-cols-2" [formGroup]="form" (ngSubmit)="save()">
        <input class="rounded-xl border border-black/10 px-4 py-2 md:col-span-2" placeholder="Company Name" formControlName="companyName" />
        <input class="rounded-xl border border-black/10 px-4 py-2 md:col-span-2" placeholder="Address" formControlName="address" />
        <input class="rounded-xl border border-black/10 px-4 py-2" placeholder="Phone" formControlName="phone" />
        <input class="rounded-xl border border-black/10 px-4 py-2" placeholder="Email" formControlName="email" />
        <input class="rounded-xl border border-black/10 px-4 py-2" placeholder="Estimate Prefix" formControlName="estimatePrefix" />
        <input class="rounded-xl border border-black/10 px-4 py-2" placeholder="Payment Prefix" formControlName="paymentPrefix" />
        <input class="rounded-xl border border-black/10 px-4 py-2" placeholder="Currency" formControlName="currency" />
        <input class="rounded-xl border border-black/10 px-4 py-2 md:col-span-2" placeholder="Logo Path" formControlName="logoPath" />
        <textarea class="rounded-xl border border-black/10 px-4 py-2 md:col-span-2" rows="5" placeholder="Default Estimate Note" formControlName="defaultEstimateNote"></textarea>
        <div class="md:col-span-2">
          <button class="rounded-full bg-brand-gold px-4 py-2 text-sm font-semibold text-black" type="submit" [disabled]="form.invalid">Save Settings</button>
        </div>
      </form>
    </section> }
  `
})
export class SettingsPageComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);
  loading = true;
  error = '';

  form = this.fb.group({
    id: [0],
    companyName: ['', Validators.required],
    address: [''],
    phone: [''],
    email: [''],
    estimatePrefix: ['EST'],
    paymentPrefix: ['PAY'],
    currency: ['INR'],
    defaultEstimateNote: [''],
    logoPath: ['']
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void { this.loading = true; this.error = ''; this.api.settings().subscribe({ next: (settings) => { this.form.patchValue(settings); this.loading = false; }, error: () => { this.error = 'Unable to load settings.'; this.loading = false; } }); }

  save(): void {
    if (this.form.invalid) return;
    this.api.saveSettings(this.form.getRawValue() as CompanySettings).subscribe((settings) => this.form.patchValue(settings));
  }
}
