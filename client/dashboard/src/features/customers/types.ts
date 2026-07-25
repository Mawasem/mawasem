export interface CustomersQuery {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  isBlocked?: boolean;
}

export interface Customer {
  id: number;
  fullNameAr: string;
  fullNameEn: string;
  phoneNumber: string;
  totalOrders: number;
  totalSpent: number;
  isBlocked: boolean;
}

export interface CustomerDetails {
  id: number;
  fullNameAr: string;
  fullNameEn: string;
  phoneNumber: string;
  email: string | null;
  birthDate: string | null;
  gender: string | null;
  referralSource: string | null;
  isBlocked: boolean;
  blockedAt: string | null;
  blockedReason: string | null;
  totalOrders: number;
  deliveredOrders: number;
  totalSpent: number;
  savedAddressCount: number;
  reviewCount: number;
}

export interface CustomersResponse {
  items: Customer[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface BlockCustomerRequest {
  reason: string;
}

export interface CustomerActionsProps {
  customer: Customer;
}

export interface BlockCustomerDialogProps {
  customer: Customer;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export interface UnblockCustomerDialogProps {
  customer: Customer;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export interface CustomerDetailsDialogProps {
  customer: Customer;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}