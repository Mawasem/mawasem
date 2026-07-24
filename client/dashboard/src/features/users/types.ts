export interface CustomersQuery {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
}

export interface Customer {
  id: number;
  fullNameAr: string;
  fullNameEn: string;
  phoneNumber: string;
  birthDate: string;
  gender: number;
  referralSource: number;
  isBlocked: boolean;
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