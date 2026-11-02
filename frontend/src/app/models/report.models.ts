export interface AllCustomerReportRow {
  customerCode: string;
  customerName: string;
  phone: string;
  location?: string | null;
  totalEstimates: number;
  estimatedValue: number;
  received: number;
  outstanding: number;
}

export interface AllEstimateReportRow {
  estimateNumber: string;
  customerName: string;
  estimateDate: string;
  status: string;
  totalAmount: number;
  received: number;
  outstanding: number;
}

export interface AllPaymentReportRow {
  paymentNumber: string;
  paymentDate: string;
  customerName: string;
  estimateNumber?: string | null;
  amount: number;
  paymentMethod: string;
  reference?: string | null;
  status: string;
}

export interface OutstandingReportRow {
  customerCode: string;
  customerName: string;
  phone: string;
  location?: string | null;
  estimatedValue: number;
  received: number;
  outstanding: number;
}

export interface MonthlyCollectionReportRow {
  month: string;
  totalReceived: number;
  totalOutstanding: number;
}
