import type { PublicProductListItem } from "./product.types"

export interface PublicProductListResponse {
  items: PublicProductListItem[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
}
