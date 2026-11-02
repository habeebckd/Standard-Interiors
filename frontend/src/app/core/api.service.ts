import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DashboardModel } from '../models/dashboard.models';
import { CustomerDetail, CustomerListResponse, CustomerStatement, CustomerUpsert, CustomerSummary } from '../models/customer.models';
import { EstimateDetail, EstimateListResponse, EstimateUpsert } from '../models/estimate.models';
import { EstimateStatus } from '../models/common.models';
import { MonthlyCollectionReportRow, OutstandingReportRow, AllCustomerReportRow, AllEstimateReportRow, AllPaymentReportRow } from '../models/report.models';
import { PaymentDetail, PaymentListResponse, PaymentReceipt, PaymentSheet, PaymentUpsert, PaymentVoid } from '../models/payment.models';
import { CompanySettings, PaymentMethod, ServiceItem, ServiceUpsert } from '../models/service.models';
import { PagingQuery, PagedResult, PaymentStatus } from '../models/common.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  readonly baseUrl = environment.apiBaseUrl;

  dashboard() {
    return this.http.get<DashboardModel>(`${this.baseUrl}/dashboard`);
  }

  customers(query: PagingQuery = {}) {
    return this.http.get<CustomerListResponse>(`${this.baseUrl}/customers`, { params: this.toParams(query) });
  }

  customer(id: number) {
    return this.http.get<CustomerDetail>(`${this.baseUrl}/customers/${id}`);
  }

  customerSummaryPdfUrl(id: number) {
    return `${this.baseUrl}/customers/${id}/summary-pdf`;
  }

  customerStatementPdfUrl(id: number) {
    return `${this.baseUrl}/customers/${id}/statement-pdf`;
  }

  customerSummaryXlsxUrl(id: number) {
    return `${this.baseUrl}/exports/customers/${id}/summary.xlsx`;
  }

  customerStatementXlsxUrl(id: number) {
    return `${this.baseUrl}/exports/customers/${id}/statement.xlsx`;
  }

  downloadFile(url: string) {
    return this.http.get(url, { responseType: 'blob' });
  }

  customerSummary(id: number) {
    return this.http.get<CustomerSummary>(`${this.baseUrl}/customers/${id}/summary`);
  }

  customerStatement(id: number) {
    return this.http.get<CustomerStatement>(`${this.baseUrl}/customers/${id}/statement`);
  }

  customerProjects(id: number) {
    return this.http.get<import('../models/customer.models').WorkProject[]>(`${this.baseUrl}/customers/${id}/projects`);
  }

  workStatusHistory(id: number) {
    return this.http.get<import('../models/customer.models').WorkStatusHistory[]>(`${this.baseUrl}/estimates/${id}/work-status-history`);
  }

  updateWorkStatus(id: number, payload: Record<string, unknown>) {
    return this.http.put<void>(`${this.baseUrl}/estimates/${id}/work-status`, payload);
  }

  startWork(id: number, payload: Record<string, unknown>) {
    return this.http.post<void>(`${this.baseUrl}/estimates/${id}/start-work`, payload);
  }

  resumeWork(id: number, payload: Record<string, unknown>) {
    return this.http.post<void>(`${this.baseUrl}/estimates/${id}/resume-work`, payload);
  }

  activateUser(id: number) { return this.http.post<void>(`${this.baseUrl}/users/${id}/activate`, {}); }
  deactivateUser(id: number) { return this.http.post<void>(`${this.baseUrl}/users/${id}/deactivate`, {}); }

  createCustomer(payload: CustomerUpsert) {
    return this.http.post<{ id: number }>(`${this.baseUrl}/customers`, payload);
  }

  updateCustomer(id: number, payload: CustomerUpsert) {
    return this.http.put<void>(`${this.baseUrl}/customers/${id}`, payload);
  }

  customerEstimates(id: number) {
    return this.http.get<PagedResult<EstimateListResponse['items'][number]>>(`${this.baseUrl}/customers/${id}/estimates`);
  }

  customerPayments(id: number) {
    return this.http.get<PagedResult<PaymentListResponse['items'][number]>>(`${this.baseUrl}/customers/${id}/payments`);
  }

  estimates(query: PagingQuery = {}) {
    return this.http.get<EstimateListResponse>(`${this.baseUrl}/estimates`, { params: this.toParams(query) });
  }

  estimate(id: number) {
    return this.http.get<EstimateDetail>(`${this.baseUrl}/estimates/${id}`);
  }

  createEstimate(payload: EstimateUpsert) {
    return this.http.post<{ id: number }>(`${this.baseUrl}/estimates`, payload);
  }

  updateEstimate(id: number, payload: EstimateUpsert) {
    return this.http.put<void>(`${this.baseUrl}/estimates/${id}`, payload);
  }

  duplicateEstimate(id: number) {
    return this.http.post<{ id: number }>(`${this.baseUrl}/estimates/${id}/duplicate`, {});
  }

  changeEstimateStatus(id: number, status: EstimateStatus) {
    return this.http.put<void>(`${this.baseUrl}/estimates/${id}/status`, { status });
  }

  estimatePdfUrl(id: number) {
    return `${this.baseUrl}/estimates/${id}/pdf`;
  }

  exportUrl(report: string) {
    return `${this.baseUrl}/exports/${report}.xlsx`;
  }

  payments(query: PagingQuery = {}) {
    return this.http.get<PaymentListResponse>(`${this.baseUrl}/payments`, { params: this.toParams(query) });
  }

  payment(id: number) {
    return this.http.get<PaymentDetail>(`${this.baseUrl}/payments/${id}`);
  }

  createPayment(payload: PaymentUpsert) {
    return this.http.post<{ id: number }>(`${this.baseUrl}/payments`, payload);
  }

  voidPayment(id: number, reason: string) {
    const payload: PaymentVoid = { reason };
    return this.http.post<void>(`${this.baseUrl}/payments/${id}/void`, payload);
  }

  paymentReceipt(id: number) {
    return this.http.get<PaymentReceipt>(`${this.baseUrl}/payments/${id}/receipt`);
  }

  paymentSheet(customerId: number) {
    return this.http.get<PaymentSheet>(`${this.baseUrl}/payments/${customerId}/sheet`);
  }

  paymentReceiptPdfUrl(id: number) {
    return `${this.baseUrl}/payments/${id}/receipt/pdf`;
  }

  paymentSheetPdfUrl(customerId: number) {
    return `${this.baseUrl}/payments/${customerId}/sheet/pdf`;
  }

  services() {
    return this.http.get<ServiceItem[]>(`${this.baseUrl}/services`);
  }

  createService(payload: ServiceUpsert) {
    return this.http.post<{ id: number }>(`${this.baseUrl}/services`, payload);
  }

  updateService(id: number, payload: ServiceUpsert) {
    return this.http.put<void>(`${this.baseUrl}/services/${id}`, payload);
  }

  paymentMethods() {
    return this.http.get<PaymentMethod[]>(`${this.baseUrl}/payment-methods`);
  }

  settings() {
    return this.http.get<CompanySettings>(`${this.baseUrl}/settings`);
  }

  saveSettings(payload: CompanySettings) {
    return this.http.put<CompanySettings>(`${this.baseUrl}/settings`, payload);
  }

  reportsCustomers() {
    return this.http.get<AllCustomerReportRow[]>(`${this.baseUrl}/reports/customers`);
  }

  reportsEstimates() {
    return this.http.get<AllEstimateReportRow[]>(`${this.baseUrl}/reports/estimates`);
  }

  reportsPayments() {
    return this.http.get<AllPaymentReportRow[]>(`${this.baseUrl}/reports/payments`);
  }

  reportsOutstanding() {
    return this.http.get<OutstandingReportRow[]>(`${this.baseUrl}/reports/outstanding`);
  }

  reportsMonthlyCollections() {
    return this.http.get<MonthlyCollectionReportRow[]>(`${this.baseUrl}/reports/monthly-collections`);
  }

  private toParams(query: PagingQuery) {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    });
    return params;
  }
}
