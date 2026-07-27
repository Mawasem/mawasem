import type { PaginatedResponse } from "@/types/pagination";

import type { ProductFormValues } from "./schema/product-form-schema";

export const ProductOptionType = {
  Standard: 1,
  Color: 2,
} as const;

export type ProductOptionType =
  (typeof ProductOptionType)[keyof typeof ProductOptionType];

export interface ProductReference {
  id: number;
  nameAr: string;
  nameEn: string;
}

export interface ProductListItem {
  id: number;
  nameAr: string;
  nameEn: string;
  slug: string;
  originalPrice: number;
  currentPrice: number;
  brand: ProductReference;
  season: ProductReference;
  isPublished: boolean;
  isFeatured: boolean;
  variantCount: number;
  totalStock: number;
  isDeleted: boolean;
  createdOn: string;
  lastModifiedOn: string | null;
}

export interface ProductSpecification {
  id: number;
  nameAr: string;
  nameEn: string;
  valueAr: string;
  valueEn: string;
}

export interface ProductDetails extends ProductListItem {
  descriptionAr: string;
  descriptionEn: string;
  categories: ProductReference[];
  collections: ProductReference[];
  grades: ProductReference[];
  tags: ProductReference[];
  specifications: ProductSpecification[];
  createdBy: string | null;
  lastModifiedBy: string | null;
  deletedOn: string | null;
  deletedBy: string | null;
}

export interface GetProductsParams {
  search?: string;
  brandId?: number;
  seasonId?: number;
  categoryId?: number;
  collectionId?: number;
  isPublished?: boolean;
  isFeatured?: boolean;
  includeDeleted?: boolean;
  pageNumber: number;
  pageSize: number;
}

export type ProductsResponse = PaginatedResponse<ProductListItem>;

export interface ProductSpecificationRequest {
  nameAr: string;
  nameEn: string;
  valueAr: string;
  valueEn: string;
}

export interface ProductPayload {
  nameAr: string;
  nameEn: string;
  descriptionAr: string;
  descriptionEn: string;
  originalPrice: number;
  currentPrice: number;
  slug: string;
  brandId: number;
  seasonId: number;
  categoryIds: number[];
  collectionIds: number[];
  gradeIds: number[];
  tagIds: number[];
  specifications: ProductSpecificationRequest[];
}

export interface UpdateProductParams {
  productId: number;
  data: ProductPayload;
}

export interface UpdateProductStatusRequest {
  isPublished: boolean;
  isFeatured: boolean;
}

export interface UpdateProductStatusParams {
  productId: number;
  data: UpdateProductStatusRequest;
}

export interface ProductOptionValue {
  id: number;
  valueAr: string;
  valueEn: string;
}

export interface ProductOption {
  id: number;
  nameAr: string;
  nameEn: string;
  type: ProductOptionType;
  values: ProductOptionValue[];
}

export interface CreateProductOptionRequest {
  nameAr: string;
  nameEn: string;
  type: ProductOptionType;
}

export interface UpdateProductOptionRequest {
  nameAr: string;
  nameEn: string;
}

export interface UpdateProductOptionParams {
  optionId: number;
  data: UpdateProductOptionRequest;
}

export interface ProductOptionValueRequest {
  valueAr: string;
  valueEn: string;
}

export interface CreateProductOptionValueParams {
  optionId: number;
  data: ProductOptionValueRequest;
}

export interface UpdateProductOptionValueParams {
  optionId: number;
  valueId: number;
  data: ProductOptionValueRequest;
}

export interface ProductVariantOption {
  optionId: number;
  optionNameAr: string;
  optionNameEn: string;
  valueId: number;
  valueAr: string;
  valueEn: string;
}

export interface ProductVariant {
  id: number;
  productId: number;
  sku: string;
  stockQuantity: number;
  isAvailable: boolean;
  canPurchase: boolean;
  rowVersion: string;
  options: ProductVariantOption[];
}

export interface CreateProductVariantRequest {
  optionValueIds: number[];
}

export interface CreateProductVariantParams {
  productId: number;
  data: CreateProductVariantRequest;
}

export interface UpdateProductVariantAvailabilityParams {
  productId: number;
  variantId: number;
  data: {
    isAvailable: boolean;
  };
}

export interface UpdateProductVariantStockParams {
  productId: number;
  variantId: number;
  data: {
    stockQuantity: number;
    rowVersion: string;
  };
}

export interface ProductImage {
  id: number;
  productId: number;
  colorOptionValueId: number | null;
  colorValueAr: string | null;
  colorValueEn: string | null;
  imageUrl: string;
  isPrimary: boolean;
  displayOrder: number;
  createdOn: string;
}

export interface UploadProductImageParams {
  productId: number;
  image: File;
  colorOptionValueId?: number;
  isPrimary: boolean;
}

export interface ProductImageMutationParams {
  productId: number;
  imageId: number;
}

export interface ReorderProductImagesParams {
  productId: number;
  data: {
    colorOptionValueId: number | null;
    imageIds: number[];
  };
}

export type ProductDialogMode = "create" | "edit";

export interface ProductDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: ProductDialogMode;
  product?: ProductListItem;
}

export interface ProductFormProps {
  mode: ProductDialogMode;
  product?: ProductDetails;
  formId: string;
  errorMessage?: string | null;
  onSubmit: (values: ProductFormValues) => Promise<void>;
}

export interface ProductActionsProps {
  product: ProductListItem;
}

export interface ProductDialogEntityProps {
  product: ProductListItem;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}
