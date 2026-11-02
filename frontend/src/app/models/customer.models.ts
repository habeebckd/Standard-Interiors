import { PagedResult } from './common.models';

export interface CustomerListItem {
  id: number;
  customerCode: string;
  name: string;
  phone: string;
  alternatePhone?: string | null;
  location?: string | null;
  totalEstimateValue: number;
  totalReceived: number;
  outstanding: number;
  estimateCount: number;
  lastActivity?: string | null;
  latestWorkStatus?: WorkStatus | number | null;
  latestWorkProgress?: number | null;
}

export interface CustomerSummary {
  id: number;
  customerCode: string;
  name: string;
  phone: string;
  alternatePhone?: string | null;
  email?: string | null;
  address?: string | null;
  location?: string | null;
  notes?: string | null;
  totalEstimated: number;
  totalReceived: number;
  outstanding: number;
  totalEstimates: number;
  totalPayments: number;
}

export interface CustomerDetail {
  id: number;
  customerCode: string;
  name: string;
  phone: string;
  alternatePhone?: string | null;
  email?: string | null;
  address?: string | null;
  location?: string | null;
  notes?: string | null;
  isActive: boolean;
  summary: CustomerSummary;
}

export interface CustomerStatementLine {
  date: string;
  reference: string;
  type: string;
  debit: number;
  credit: number;
  runningBalance: number;
}

export interface CustomerStatement {
  summary: CustomerSummary;
  lines: CustomerStatementLine[];
}

export interface CustomerUpsert {
  name: string;
  phone: string;
  alternatePhone?: string | null;
  email?: string | null;
  address?: string | null;
  location?: string | null;
  notes?: string | null;
  isActive?: boolean;
}

export interface CustomerListResponse extends PagedResult<CustomerListItem> {}

export type WorkStatus = 'NotStarted' | 'Scheduled' | 'InProgress' | 'TemporarilyStopped' | 'PermanentlyStopped' | 'Completed' | 'Cancelled';
export interface WorkProject {
  estimateId: number; estimateNumber: string; customerName: string; projectName?: string | null; status: WorkStatus | number; progressPercentage: number;
  plannedStartDate?: string | null; actualStartDate?: string | null; plannedEndDate?: string | null; actualEndDate?: string | null;
  description?: string | null; stopDate?: string | null; stopReason?: string | null; resumeDate?: string | null; resumeNote?: string | null; lastUpdatedAt: string; lastUpdatedBy?: string | null;
}
export interface WorkStatusHistory { id: number; previousStatus?: WorkStatus | number | null; newStatus: WorkStatus | number; progressPercentage: number; changedAt: string; changedBy?: string | null; description?: string | null; stopReason?: string | null; stopDate?: string | null; resumeDate?: string | null; }
