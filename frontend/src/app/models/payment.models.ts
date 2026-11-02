import { PaymentStatus, PagedResult } from './common.models';

export interface PaymentUpsert {
  customerId: number;
  estimateId?: number | null;
  paymentDate: string;
  amount: number;
  paymentMethodId: number;
  transactionReference?: string | null;
  notes?: string | null;
  allowOverpayment?: boolean;
}

export interface PaymentListItem {
  id: number;
  paymentNumber: string;
  paymentDate: string;
  customerName: string;
  estimateNumber?: string | null;
  amount: number;
  paymentMethodName: string;
  transactionReference?: string | null;
  notes?: string | null;
  status: PaymentStatus;
}

export interface PaymentDetail extends PaymentListItem {
  customerId: number;
  paymentMethodId: number;
  voidedAt?: string | null;
  voidReason?: string | null;
}

export interface PaymentReceipt {
  receiptNumber: string;
  paymentDate: string;
  customerName: string;
  estimateNumber?: string | null;
  previousBalance: number;
  amountReceived: number;
  remainingBalance: number;
  paymentMethod: string;
  transactionReference?: string | null;
  note?: string | null;
}

export interface PaymentSheet {
  customerName: string;
  estimateNumber?: string | null;
  totalPrice: number;
  totalPaid: number;
  remainingBalance: number;
  payments: PaymentListItem[];
}

export interface PaymentVoid {
  reason: string;
}

export interface PaymentListResponse extends PagedResult<PaymentListItem> {}
