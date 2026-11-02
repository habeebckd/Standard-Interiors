export interface PagingQuery {
  page?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
  sortDesc?: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export type EstimatePricingMode = 1 | 2 | 3;
export type EstimateStatus = 'Draft' | 'Sent' | 'Approved' | 'Rejected' | 'Cancelled' | 'Completed';
export type PaymentStatus = 'Posted' | 'Voided';
