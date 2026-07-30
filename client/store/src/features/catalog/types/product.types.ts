export interface PublicBrandReference {
  id: number
  nameEn: string
  nameAr: string
  logoUrl: string | null
}

export interface PublicSeasonReference {
  id: number
  nameEn: string
  nameAr: string
  isActive: boolean
}

export interface PublicProductListItem {
  id: number
  slug: string
  nameEn: string
  nameAr: string
  originalPrice: number
  currentPrice: number
  discountPercentage: number
  isFeatured: boolean
  isInStock: boolean
  canPurchase: boolean
  primaryImageUrl: string | null
  brand: PublicBrandReference
  season: PublicSeasonReference
}
