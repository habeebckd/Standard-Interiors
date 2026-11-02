import { EstimatePricingMode, EstimateStatus, PagedResult } from './common.models';
import { CustomerSummary } from './customer.models';

export interface EstimateItem {
  id?: number | null;
  serviceId?: number | null;
  serviceName?: string | null;
  description: string;
  quantity?: number | null;
  unit?: string | null;
  rate?: number | null;
  manualAmount?: number | null;
  notes?: string | null;
  displayOrder: number;
  isCustomItem: boolean;
}

export interface EstimateSection {
  id?: number | null;
  sectionNumber: string;
  title: string;
  description?: string | null;
  pricingMode?: EstimatePricingMode | null;
  manualSectionAmount?: number | null;
  displayOrder: number;
  items: EstimateItem[];
}

export interface EstimateUpsert {
  customerId: number;
  estimateDate: string;
  validUntil?: string | null;
  pricingMode: EstimatePricingMode;
  manualOverallAmount?: number | null;
  status: EstimateStatus;
  notes?: string | null;
  estimateNote?: string | null;
  sections: EstimateSection[];
}

export interface EstimateListItem {
  id: number;
  estimateNumber: string;
  customerId: number;
  customerName: string;
  customerPhone: string;
  estimateDate: string;
  sectionCount: number;
  totalAmount: number;
  receivedAmount: number;
  balance: number;
  status: EstimateStatus;
  pricingMode: EstimatePricingMode;
}

export interface EstimateDetail {
  id: number;
  estimateNumber: string;
  customer: CustomerSummary;
  estimateDate: string;
  validUntil?: string | null;
  pricingMode: EstimatePricingMode;
  manualOverallAmount?: number | null;
  status: EstimateStatus;
  notes?: string | null;
  estimateNote?: string | null;
  revisionNumber: number;
  sections: EstimateSection[];
  totalAmount: number;
  receivedAmount: number;
  balance: number;
}

export interface EstimateListResponse extends PagedResult<EstimateListItem> {}
