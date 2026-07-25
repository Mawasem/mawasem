export interface Collection {
  id: number;
  nameAr: string;
  nameEn: string;
  seasonId: number;
  seasonNameAr: string;
  seasonNameEn: string;
  isDeleted: boolean;
}

export interface CollectionPayload {
  nameAr: string;
  nameEn: string;
  seasonId: number;
}

export interface UpdateCollectionParams {
  id: number;
  data: CollectionPayload;
}

export interface CollectionQueryParams {
  search?: string;
  includeDeleted?: boolean;
  pageNumber?: number;
  pageSize?: number;
}