import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable, finalize } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { CustomerListItem } from '../../models/customer.models';
import { EstimateDetail, EstimateItem, EstimateSection, EstimateUpsert } from '../../models/estimate.models';
import { PaymentMethod, ServiceItem } from '../../models/service.models';
import { money } from '../../shared/format';
import { PageHeaderComponent } from '../../shared/page-header.component';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, PageHeaderComponent],
  template: `
    <app-page-header [title]="editing ? 'Edit Estimate' : 'Create Estimate'" subtitle="Build sections, items and totals in a single place.">
      <div actions class="flex gap-2">
        <a routerLink="/estimates" class="rounded-full border border-black/10 px-4 py-2 text-sm">Back</a>
         <button class="rounded-full bg-brand-gold px-4 py-2 text-sm font-semibold text-black" (click)="save()" [disabled]="saving || form.invalid">{{ saving ? 'Saving...' : 'Save Estimate' }}</button>
      </div>
    </app-page-header>

     @if (error) { <div class="notice notice-error mb-4">{{ error }}</div> }
     <form [formGroup]="form" class="grid gap-6 xl:grid-cols-[1.1fr_0.9fr]">
      <section class="space-y-4 rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
        <div class="grid gap-3 md:grid-cols-2 lg:grid-cols-3">
          <select class="rounded-xl border border-black/10 px-4 py-2" formControlName="customerId">
            <option value="">Select customer</option>
            @for (c of customers; track c.id) { <option [value]="c.id">{{ c.customerCode }} - {{ c.name }}</option> }
          </select>
          <input class="rounded-xl border border-black/10 px-4 py-2" type="date" formControlName="estimateDate" />
          <select class="rounded-xl border border-black/10 px-4 py-2" formControlName="pricingMode">
            <option [value]="1">Item Based</option>
            <option [value]="2">Section Based</option>
            <option [value]="3">Overall</option>
          </select>
        </div>

        <div class="grid gap-3 md:grid-cols-2">
          <input class="rounded-xl border border-black/10 px-4 py-2" placeholder="Estimate Note" formControlName="estimateNote" />
          <input class="rounded-xl border border-black/10 px-4 py-2" placeholder="Manual Overall Amount" formControlName="manualOverallAmount" />
        </div>

        <div formArrayName="sections" [formGroup]="form" class="space-y-4">
          @for (section of sections.controls; track $index; let i = $index) {
            <div [formGroupName]="i" class="rounded-2xl border border-black/10 bg-brand-paper p-4">
              <div class="flex flex-wrap items-center justify-between gap-3">
                <div class="grid flex-1 gap-3 md:grid-cols-4">
                  <input class="rounded-xl border border-black/10 px-3 py-2" placeholder="Section Number" formControlName="sectionNumber" />
                  <input class="rounded-xl border border-black/10 px-3 py-2 md:col-span-2" placeholder="Section Title" formControlName="title" />
                  <input class="rounded-xl border border-black/10 px-3 py-2" placeholder="Section Amount" formControlName="manualSectionAmount" />
                </div>
                <div class="flex gap-2 text-xs">
                  <button type="button" class="rounded-full border px-3 py-1" (click)="moveSection(i, -1)">Up</button>
                  <button type="button" class="rounded-full border px-3 py-1" (click)="moveSection(i, 1)">Down</button>
                  <button type="button" class="rounded-full border border-red-200 px-3 py-1 text-red-700" (click)="removeSection(i)">Delete</button>
                </div>
              </div>

              <div class="mt-3 space-y-3" formArrayName="items">
                @for (item of items(i).controls; track $index; let j = $index) {
                  <div [formGroupName]="j" class="grid gap-2 rounded-xl border border-black/10 bg-white p-3 md:grid-cols-6">
                    <select class="rounded-lg border border-black/10 px-3 py-2 md:col-span-2" formControlName="serviceId">
                      <option value="">Custom / Unlinked</option>
                      @for (srv of services; track srv.id) { <option [value]="srv.id">{{ srv.serviceName }}</option> }
                    </select>
                    <input class="rounded-lg border border-black/10 px-3 py-2 md:col-span-2" placeholder="Description" formControlName="description" />
                    <input class="rounded-lg border border-black/10 px-3 py-2" placeholder="Qty" formControlName="quantity" />
                    <input class="rounded-lg border border-black/10 px-3 py-2" placeholder="Amount" formControlName="manualAmount" />
                  </div>
                }
              </div>

              <div class="mt-3 flex gap-2">
                <button type="button" class="rounded-full border border-black/10 px-3 py-2 text-sm" (click)="addItem(i)">+ Add Item</button>
                <button type="button" class="rounded-full border border-black/10 px-3 py-2 text-sm" (click)="addCustomItem(i)">+ Add Custom</button>
              </div>
            </div>
          }
        </div>

        <button type="button" class="rounded-full border border-black/10 px-4 py-2 text-sm" (click)="addSection()">+ Add Section</button>
      </section>

      <aside class="space-y-4">
        <div class="rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
          <h3 class="text-lg font-semibold">Project Total</h3>
          <div class="mt-2 text-3xl font-semibold text-brand-gold">{{ money(projectTotal()) }}</div>
          <p class="mt-2 text-sm text-black/55">Backend validates the authoritative totals when the estimate is saved.</p>
        </div>

        <div class="rounded-2xl border border-black/10 bg-white p-5 shadow-sm">
          <h3 class="text-lg font-semibold">PDF Preview</h3>
          <p class="mt-2 text-sm text-black/55">After saving, generate the estimate PDF from the backend route.</p>
        </div>
      </aside>
    </form>
  `
})
export class EstimateBuilderPageComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  editing = false;
  saving = false;
  error = '';
  customers: CustomerListItem[] = [];
  services: ServiceItem[] = [];
  paymentMethods: PaymentMethod[] = [];
  form = this.fb.group({
    customerId: ['', Validators.required],
    estimateDate: [new Date().toISOString().slice(0, 10), Validators.required],
    pricingMode: [2, Validators.required],
    manualOverallAmount: [''],
    status: ['Draft', Validators.required],
    notes: [''],
    estimateNote: [''],
    sections: this.fb.array([])
  });

  get sections(): FormArray { return this.form.get('sections') as FormArray; }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.editing = !!id;

    this.api.customers({ pageSize: 200 }).subscribe((res) => this.customers = res.items);
    this.api.services().subscribe((res) => this.services = res);
    this.api.paymentMethods().subscribe((res) => this.paymentMethods = res);

    if (id) {
      this.api.estimate(id).subscribe((estimate) => this.patchEstimate(estimate));
    } else {
      this.addSection();
    }
  }

  addSection(section?: Partial<EstimateSection>): void {
    this.sections.push(this.fb.group({
      sectionNumber: [section?.sectionNumber || `${this.sections.length + 1}`],
      title: [section?.title || 'New Section', Validators.required],
      description: [section?.description || ''],
      pricingMode: [section?.pricingMode ?? null],
      manualSectionAmount: [section?.manualSectionAmount ?? ''],
      displayOrder: [section?.displayOrder || this.sections.length + 1],
      items: this.fb.array([])
    }));
    this.addItem(this.sections.length - 1);
  }

  items(index: number): FormArray {
    return this.sections.at(index).get('items') as FormArray;
  }

  addItem(sectionIndex: number): void {
    this.items(sectionIndex).push(this.fb.group({
      serviceId: [''],
      description: ['', Validators.required],
      quantity: [''],
      unit: [''],
      rate: [''],
      manualAmount: [''],
      notes: [''],
      displayOrder: [this.items(sectionIndex).length + 1],
      isCustomItem: [false]
    }));
  }

  addCustomItem(sectionIndex: number): void {
    this.items(sectionIndex).push(this.fb.group({
      serviceId: [null],
      description: ['Custom Work Item', Validators.required],
      quantity: [''],
      unit: [''],
      rate: [''],
      manualAmount: [''],
      notes: [''],
      displayOrder: [this.items(sectionIndex).length + 1],
      isCustomItem: [true]
    }));
  }

  removeSection(index: number): void { this.sections.removeAt(index); }
  moveSection(index: number, delta: number): void { this.swap(this.sections, index, index + delta); }
  moveItem(sectionIndex: number, index: number, delta: number): void { this.swap(this.items(sectionIndex), index, index + delta); }

  save(): void {
    if (this.form.invalid || this.saving) return;
    this.saving = true;
    this.error = '';
    const raw = this.form.getRawValue() as any;
    const payload: EstimateUpsert = {
      customerId: Number(raw.customerId),
      estimateDate: raw.estimateDate,
      validUntil: raw.validUntil ?? null,
      pricingMode: Number(raw.pricingMode) as any,
      manualOverallAmount: raw.manualOverallAmount === '' || raw.manualOverallAmount == null ? null : Number(raw.manualOverallAmount),
      status: raw.status,
      notes: raw.notes,
      estimateNote: raw.estimateNote,
      sections: raw.sections.map((section: any, index: number) => ({
        sectionNumber: section.sectionNumber,
        title: section.title,
        description: section.description,
        pricingMode: section.pricingMode === '' || section.pricingMode == null ? null : Number(section.pricingMode),
        manualSectionAmount: section.manualSectionAmount === '' || section.manualSectionAmount == null ? null : Number(section.manualSectionAmount),
        displayOrder: index + 1,
        items: (section.items || []).map((item: any, itemIndex: number) => ({
          id: item.id,
          serviceId: item.serviceId === '' || item.serviceId == null ? null : Number(item.serviceId),
          serviceName: item.serviceName,
          description: item.description,
          quantity: item.quantity === '' || item.quantity == null ? null : Number(item.quantity),
          unit: item.unit,
          rate: item.rate === '' || item.rate == null ? null : Number(item.rate),
          manualAmount: item.manualAmount === '' || item.manualAmount == null ? null : Number(item.manualAmount),
          notes: item.notes,
          displayOrder: itemIndex + 1,
          isCustomItem: !!item.isCustomItem
        }))
      }))
    };
    const request: Observable<unknown> = this.editing
      ? this.api.updateEstimate(Number(this.route.snapshot.paramMap.get('id')), payload)
      : this.api.createEstimate(payload);
    request.pipe(finalize(() => this.saving = false)).subscribe({
      next: () => this.router.navigateByUrl('/estimates'),
      error: (error: HttpErrorResponse) => this.error = error.error?.detail || error.error?.title || (error.status === 0 ? 'Unable to reach the API.' : `Request failed (${error.status}).`)
    });
  }

  projectTotal(): number {
    return this.sections.controls.reduce((sum, section) => {
      const sectionAmount = Number(section.get('manualSectionAmount')?.value || 0);
      return sum + sectionAmount;
    }, 0);
  }

  private patchEstimate(estimate: EstimateDetail): void {
    this.form.patchValue({
      customerId: String(estimate.customer.id),
      estimateDate: estimate.estimateDate.slice(0, 10),
      pricingMode: estimate.pricingMode,
      manualOverallAmount: estimate.manualOverallAmount == null ? '' : String(estimate.manualOverallAmount),
      status: estimate.status,
      notes: estimate.notes ?? '',
      estimateNote: estimate.estimateNote ?? ''
    });

    this.sections.clear();
    estimate.sections.forEach((section) => {
      this.addSection(section);
      const sectionForm = this.sections.at(this.sections.length - 1);
      sectionForm.patchValue({
        sectionNumber: section.sectionNumber,
        title: section.title,
        description: section.description ?? '',
        pricingMode: section.pricingMode,
        manualSectionAmount: section.manualSectionAmount ?? '',
        displayOrder: section.displayOrder
      });
      const items = this.items(this.sections.length - 1);
      items.clear();
      section.items.forEach((item) => items.push(this.fb.group({
        serviceId: [item.serviceId ?? ''],
        description: [item.description, Validators.required],
        quantity: [item.quantity ?? ''],
        unit: [item.unit ?? ''],
        rate: [item.rate ?? ''],
        manualAmount: [item.manualAmount ?? ''],
        notes: [item.notes ?? ''],
        displayOrder: [item.displayOrder],
        isCustomItem: [item.isCustomItem]
      })));
    });
  }

  private swap(array: FormArray, from: number, to: number): void {
    if (to < 0 || to >= array.length) return;
    const current = array.at(from);
    array.removeAt(from);
    array.insert(to, current);
  }

  protected readonly money = money;
}
