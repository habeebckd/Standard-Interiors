import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { ThemeService } from '../core/theme.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="app-shell min-h-screen bg-brand-cream text-brand-ink">
      <div class="flex min-h-screen">
        @if (mobileOpen) { <button class="drawer-backdrop lg:hidden" aria-label="Close menu" (click)="mobileOpen = false"></button> }
        <aside class="sidebar" [class.sidebar-open]="mobileOpen">
          <div class="sidebar-brand">
            <div class="brand-mark" aria-hidden="true"><img src="/standard-interiors-logo.png" alt="" /></div>
            <div>
              <div class="text-[10px] uppercase tracking-[0.35em] text-white/60">Standard</div>
              <div class="text-xl font-semibold tracking-wide text-brand-gold">Interiors</div>
            </div>
          </div>

          <nav class="sidebar-nav">
            @for (item of navItems; track item.path) {
              @if (!item.adminOnly || auth.isAdmin()) {
                <a [routerLink]="item.path" routerLinkActive="active" class="sidebar-link" (click)="mobileOpen = false">
                  <span class="nav-dot"></span>{{ item.label }}
                </a>
              }
            }
          </nav>
          <div class="sidebar-footer">Internal operations workspace</div>
        </aside>

        <main class="flex-1">
          <header class="topbar">
            <div class="flex items-center justify-between gap-4">
              <button class="menu-button lg:hidden" aria-label="Open menu" (click)="mobileOpen = true">☰</button>
              <div class="hidden text-xs uppercase tracking-[0.24em] text-black/45 sm:block">Operations</div>
              <div class="topbar-actions">
                <button class="theme-toggle" (click)="theme.toggle()" [attr.aria-label]="theme.dark() ? 'Switch to light theme' : 'Switch to dark theme'">{{ theme.dark() ? '☀' : '☾' }}<span>{{ theme.dark() ? 'Light' : 'Dark' }}</span></button>
                <div class="user-chip"><span class="user-avatar">{{ auth.user()?.displayName?.charAt(0) || 'A' }}</span><span class="hidden sm:block"><strong>{{ auth.user()?.displayName }}</strong><small>{{ auth.user()?.role }}</small></span></div>
                <button class="button button-secondary" (click)="logout()">Logout</button>
              </div>
            </div>
          </header>

          <div class="px-4 py-6 lg:px-8">
            <router-outlet />
          </div>
        </main>
      </div>
    </div>
  `
})
export class AppShellComponent {
  protected readonly navItems = [
    { label: 'Dashboard', path: '/dashboard', adminOnly: false },
    { label: 'Customers', path: '/customers', adminOnly: false },
    { label: 'Estimates', path: '/estimates', adminOnly: false },
    { label: 'Payments', path: '/payments', adminOnly: false },
    { label: 'Services', path: '/services', adminOnly: true },
    { label: 'Reports', path: '/reports', adminOnly: false },
    { label: 'Settings', path: '/settings', adminOnly: true },
    { label: 'Users', path: '/users', adminOnly: true }
  ];

  protected readonly auth = inject(AuthService);
  protected readonly theme = inject(ThemeService);
  mobileOpen = false;
  logout(): void { this.auth.logout(); window.location.href = '/login'; }
}
