export enum PublicProductSortOption {
  Newest = 1,
  PriceLowToHigh = 2,
  PriceHighToLow = 3,
}

export interface GetPublicProductsParams {
  searchTerm?: string
  seasonId?: number
  collectionId?: number
  categoryId?: number
  brandId?: number
  gradeId?: number
  tagId?: number
  minimumPrice?: number
  maximumPrice?: number
  inStockOnly?: boolean
  isFeatured?: boolean
  sortBy?: PublicProductSortOption
  pageNumber?: number
  pageSize?: number
}
