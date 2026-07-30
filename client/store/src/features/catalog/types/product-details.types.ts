import type {
  PublicBrandReference,
  PublicSeasonReference,
} from "./product.types"

export interface PublicNamedReference {
  id: number
  nameEn: string
  nameAr: string
}

export interface PublicProductImage {
  id: number
  imageUrl: string
  isPrimary: boolean
  displayOrder: number
  colorOptionValueId: number | null
}

export interface PublicProductSpecification {
  id: number
  nameEn: string
  nameAr: string
  valueEn: string
  valueAr: string
}

export interface PublicProductOptionValue {
  id: number
  valueEn: string
  valueAr: string
}

export interface PublicProductOption {
  id: number
  nameEn: string
  nameAr: string
  type: number
  values: PublicProductOptionValue[]
}

export interface PublicProductVariantOption {
  optionId: number
  optionValueId: number
}

export interface PublicProductVariant {
  id: number
  sku: string
  stockQuantity: number
  isAvailable: boolean
  isInStock: boolean
  canPurchase: boolean
  options: PublicProductVariantOption[]
}

export interface PublicProductReview {
  id: number
  rating: number
  comment: string
  reviewerNameEn: string
  reviewerNameAr: string
  createdOn: string
}

export interface PublicProductDetails {
  id: number
  slug: string
  nameEn: string
  nameAr: string
  descriptionEn: string
  descriptionAr: string
  originalPrice: number
  currentPrice: number
  discountPercentage: number
  isFeatured: boolean
  isInStock: boolean
  canPurchase: boolean
  primaryImageUrl: string | null
  brand: PublicBrandReference
  season: PublicSeasonReference
  categories: PublicNamedReference[]
  collections: PublicNamedReference[]
  grades: PublicNamedReference[]
  tags: PublicNamedReference[]
  specifications: PublicProductSpecification[]
  images: PublicProductImage[]
  options: PublicProductOption[]
  variants: PublicProductVariant[]
  averageRating: number
  reviewCount: number
  reviews: PublicProductReview[]
}
