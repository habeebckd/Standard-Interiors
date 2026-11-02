import { Routes } from '@angular/router';
import { AppShellComponent } from './layout/app-shell.component';
import { DashboardPageComponent } from './features/dashboard/dashboard-page.component';
import { CustomersPageComponent } from './features/customers/customers-page.component';
import { CustomerDetailPageComponent } from './features/customers/customer-detail-page.component';
import { EstimatesPageComponent } from './features/estimates/estimates-page.component';
import { EstimateBuilderPageComponent } from './features/estimates/estimate-builder-page.component';
import { PaymentsPageComponent } from './features/payments/payments-page.component';
import { ServicesPageComponent } from './features/services/services-page.component';
import { ReportsPageComponent } from './features/reports/reports-page.component';
import { SettingsPageComponent } from './features/settings/settings-page.component';
import { LoginPageComponent } from './features/auth/login-page.component';
import { UsersPageComponent } from './features/users/users-page.component';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginPageComponent },
  {
    path: '',
    component: AppShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      { path: 'dashboard', component: DashboardPageComponent },
      { path: 'customers', component: CustomersPageComponent },
      { path: 'customers/:id', component: CustomerDetailPageComponent },
      { path: 'estimates', component: EstimatesPageComponent },
      { path: 'estimates/new', component: EstimateBuilderPageComponent },
      { path: 'estimates/:id', component: EstimateBuilderPageComponent },
      { path: 'payments', component: PaymentsPageComponent },
      { path: 'services', component: ServicesPageComponent },
      { path: 'reports', component: ReportsPageComponent },
      { path: 'settings', component: SettingsPageComponent }
      ,{ path: 'users', component: UsersPageComponent }
    ]
  }
];
