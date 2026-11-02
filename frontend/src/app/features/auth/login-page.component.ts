import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <main class="flex min-h-screen items-center justify-center bg-brand-charcoal px-4 py-8">
      <section class="w-full max-w-md rounded-3xl border border-white/10 bg-brand-paper p-8 shadow-2xl">
        <div class="flex flex-col items-center text-center">
          <img src="/standard-interiors-logo.png" alt="Standard Interiors" class="h-28 w-28 rounded-full object-cover shadow-lg" />
          <p class="mt-5 text-xs uppercase tracking-[0.35em] text-brand-gold">Standard Interiors</p>
          <h1 class="mt-2 text-2xl font-semibold text-brand-charcoal">Internal Business System</h1>
          <p class="mt-2 text-sm text-black/55">Sign in to manage estimates, customers, and collections.</p>
        </div>
        <form class="mt-8 space-y-4" [formGroup]="form" (ngSubmit)="submit()">
          <label class="block text-sm font-medium">Username<input class="mt-1 w-full rounded-xl border border-black/10 bg-white px-4 py-3 outline-none focus:border-brand-gold" formControlName="username" autocomplete="username" /></label>
          <label class="block text-sm font-medium">Password<input class="mt-1 w-full rounded-xl border border-black/10 bg-white px-4 py-3 outline-none focus:border-brand-gold" type="password" formControlName="password" autocomplete="current-password" /></label>
          @if (error) { <p class="rounded-xl bg-red-50 p-3 text-sm text-red-700">{{ error }}</p> }
          <button class="w-full rounded-full bg-brand-gold px-5 py-3 font-semibold text-black" type="submit" [disabled]="form.invalid || loading">{{ loading ? 'Signing in...' : 'Sign in' }}</button>
        </form>
      </section>
    </main>
  `
})
export class LoginPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  readonly form = this.fb.group({ username: ['', Validators.required], password: ['', Validators.required] });
  loading = false;
  error = '';

  submit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    this.error = '';
    this.auth.login(this.form.value.username ?? '', this.form.value.password ?? '').subscribe({
      next: () => this.router.navigateByUrl('/dashboard'),
      error: (error) => { this.error = error.status === 401 ? 'Invalid username or password.' : 'Unable to sign in.'; this.loading = false; }
    });
  }
}
