import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { ServiceItem, ServiceUpsert } from '../../models/service.models';
import { PageHeaderComponent } from '../../shared/page-header.component';
import { money } from '../../shared/format';
import { StatusBadgeComponent } from '../../shared/status-badge.component';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent, StatusBadgeComponent],
  template: `
    <app-page-header title="Services" subtitle="Maintain master work types without hardcoding your catalog."></app-page-header>

    @if (loading) { <div class="loading-state"><span class="spinner"></span>Loading services...</div> } @else if (error) { <div class="notice notice-error"><span>{{ error }}</span><button class="underline" (click)="load()">Retry</button></div> } @else { <div class="services-layout">
      <section class="rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
        <div class="service-table-wrapper">
        <table class="data-table services-table text-left text-sm">
          <thead class="text-xs uppercase tracking-wide text-black/45">
            <tr><th class="py-2">Code</th><th>Name</th><th>Display</th><th>Price</th><th>Status</th></tr>
          </thead>
          <tbody>
            @for (s of services; track s.id) {
              <tr class="border-t border-black/5">
                <td class="py-2 font-medium">{{ s.serviceCode }}</td>
                <td>{{ s.serviceName }}</td>
                <td>{{ s.displayOrder }}</td>
                <td>{{ s.defaultPrice ? money(s.defaultPrice) : '-' }}</td>
                <td><app-status-badge [label]="s.isActive ? 'Active' : 'Inactive'" [tone]="s.isActive ? 'green' : 'red'"></app-status-badge></td>
              </tr>
            }
          </tbody>
        </table>
        </div>
      </section>

      <section class="content-card services-form-card p-6">
        <div><p class="eyebrow">Service catalog</p><h3 class="text-xl font-semibold">New Service</h3></div>
        <form class="service-form mt-5" [formGroup]="form" (ngSubmit)="save()">
          <label class="field-label">Service Name <span>*</span><input class="field" placeholder="Enter service name" formControlName="serviceName" /></label>
          <label class="field-label">Description<input class="field" placeholder="Enter description" formControlName="description" /></label>
          <label class="field-label">Default Unit<input class="field" placeholder="e.g. sq ft, pcs, job" formControlName="defaultUnit" /></label>
          <label class="field-label">Default Price<input class="field" type="number" placeholder="0.00" formControlName="defaultPrice" /></label>
          <label class="field-label">Display Order<input class="field" type="number" placeholder="1" formControlName="displayOrder" /></label>
         @if (saveError) { <div class="notice notice-error">{{ saveError }}</div> }<div class="service-form-actions"><button class="button button-primary service-save-button" type="submit" [disabled]="saving || form.invalid">{{ saving ? 'Saving...' : 'Save Service' }}</button></div>
        </form>
      </section>
    </div> }
  `
})
export class ServicesPageComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);

  services: ServiceItem[] = [];
  loading = true;
  error = '';
  saveError = '';
  saving = false;
  form = this.fb.group({
    serviceName: ['', Validators.required],
    description: [''],
    defaultUnit: [''],
    defaultPrice: [''],
    displayOrder: [1, Validators.required]
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true; this.error = '';
    this.api.services().subscribe({ next: (res) => { this.services = res || []; this.loading = false; }, error: () => { this.error = 'Unable to load services.'; this.loading = false; } });
  }

  save(): void {
    if (this.form.invalid || this.saving) return;
    this.saving = true;
    this.saveError = '';
    const raw = this.form.getRawValue() as any;
    const payload: ServiceUpsert = {
      serviceName: raw.serviceName,
      description: raw.description,
      defaultUnit: raw.defaultUnit,
      defaultPrice: raw.defaultPrice === '' || raw.defaultPrice == null ? null : Number(raw.defaultPrice),
      displayOrder: Number(raw.displayOrder),
      isActive: true
    };
    this.api.createService(payload).pipe(finalize(() => this.saving = false)).subscribe({
      next: () => {
        this.form.reset({ serviceName: '', description: '', defaultUnit: '', defaultPrice: '', displayOrder: 1 });
        this.load();
      },
      error: (error: HttpErrorResponse) => this.saveError = error.error?.detail || error.error?.title || (error.status === 0 ? 'Unable to reach the API.' : `Request failed (${error.status}).`)
    });
  }

  protected readonly money = money;
}
