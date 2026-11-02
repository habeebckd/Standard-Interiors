export interface ServiceItem {
  id: number;
  serviceCode: string;
  serviceName: string;
  description?: string | null;
  defaultUnit?: string | null;
  defaultPrice?: number | null;
  displayOrder: number;
  isActive: boolean;
}

export interface ServiceUpsert {
  serviceName: string;
  description?: string | null;
  defaultUnit?: string | null;
  defaultPrice?: number | null;
  displayOrder: number;
  isActive?: boolean;
}

export interface PaymentMethod {
  id: number;
  code: string;
  name: string;
  isActive: boolean;
  displayOrder: number;
}

export interface CompanySettings {
  id: number;
  companyName: string;
  address?: string | null;
  phone?: string | null;
  email?: string | null;
  estimatePrefix: string;
  paymentPrefix: string;
  currency: string;
  defaultEstimateNote: string;
  logoPath?: string | null;
}
