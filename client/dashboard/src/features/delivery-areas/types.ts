import type { PaginatedResponse } from "@/types/pagination";

import type { DeliveryAreaFormValues } from "./schema/delivery-area-form-schema";

export const DeliveryAreaStatus = {
  Pending: 1,
  Confirmed: 2,
  Restricted: 3,
} as const;

export type DeliveryAreaStatus =
  (typeof DeliveryAreaStatus)[keyof typeof DeliveryAreaStatus];

export interface DeliveryArea {
  id: number;
  nameAr: string;
  nameEn: string;
  status: DeliveryAreaStatus;
  deliveryFee: number;
  effectiveDeliveryFee: number;
  isFreeDelivery: boolean;
  isActive: boolean;
  activeAddressCount: number;
  isDeleted: boolean;
  createdOn: string;
  createdBy: string | null;
  lastModifiedOn: string | null;
  lastModifiedBy: string | null;
  deletedOn: string | null;
  deletedBy: string | null;
}

export interface GetDeliveryAreasParams {
  search?: string;
  status?: DeliveryAreaStatus;
  isActive?: boolean;
  includeDeleted?: boolean;
  pageNumber: number;
  pageSize: number;
}

export type DeliveryAreasResponse = PaginatedResponse<DeliveryArea>;

export interface CreateDeliveryAreaRequest {
  nameAr: string;
  nameEn: string;
  deliveryFee: number;
  isFreeDelivery: boolean;
  isActive: boolean;
  status: DeliveryAreaStatus;
}

export interface UpdateDeliveryAreaRequest {
  nameAr: string;
  nameEn: string;
  deliveryFee: number;
  isFreeDelivery: boolean;
  isActive: boolean;
}

export interface UpdateDeliveryAreaParams {
  deliveryAreaId: number;
  data: UpdateDeliveryAreaRequest;
}

export interface UpdateDeliveryAreaStatusRequest {
  status: DeliveryAreaStatus;
}

export interface UpdateDeliveryAreaStatusParams {
  deliveryAreaId: number;
  data: UpdateDeliveryAreaStatusRequest;
}

export type DeliveryAreaDialogMode = "create" | "edit";

export interface DeliveryAreaDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: DeliveryAreaDialogMode;
  deliveryArea?: DeliveryArea;
}

export interface DeliveryAreaFormProps {
  mode: DeliveryAreaDialogMode;
  deliveryArea?: DeliveryArea;
  formId: string;
  errorMessage?: string | null;
  onSubmit: (values: DeliveryAreaFormValues) => Promise<void>;
}

export interface DeliveryAreaActionsProps {
  deliveryArea: DeliveryArea;
}

export interface DeliveryAreaDetailsDialogProps {
  deliveryArea: DeliveryArea;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export interface DeliveryAreaStatusDialogProps {
  deliveryArea: DeliveryArea;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export interface DeliveryAreaMutationDialogProps {
  deliveryArea: DeliveryArea;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}
