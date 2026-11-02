export interface DashboardModel {
  totalCustomers: number;
  totalEstimates: number;
  totalEstimatedValue: number;
  totalAmountReceived: number;
  totalOutstandingBalance: number;
  thisMonthReceived: number;
  monthlyPaymentTrend: Array<{ month: string; amountReceived: number }>;
  recentEstimates: Array<{ estimateNumber: string; customer: string; date: string; amount: number; status: string; balance: number }>;
  recentPayments: Array<{ date: string; customer: string; amount: number; method: string; reference?: string | null }>;
  outstandingCustomers: Array<{ customer: string; estimateValue: number; received: number; balance: number }>;
  workStatusSummary?: Array<{ status: string | number; count: number }>;
}
