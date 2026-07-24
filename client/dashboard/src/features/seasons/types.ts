import type { PaginatedResponse } from "@/types/pagination";
import type { SeasonFormValues } from "./schema/season-schema";

export interface Season {
  id: number;
  nameAr: string;
  nameEn: string;
  descriptionAr: string;
  descriptionEn: string;
  isActive: boolean;
  isDeleted: boolean;
  productCount: number;
}

export interface SeasonPayload {
  nameAr: string;
  nameEn: string;
  descriptionAr: string;
  descriptionEn: string;
  isActive: boolean;
}

export interface UpdateSeasonParams {
  id: number;
  data: SeasonPayload;
}

export interface SeasonQueryParams {
  search?: string;
  isActive?: boolean;
  includeDeleted?: boolean;
  pageNumber: number;
  pageSize: number;
}

export type SeasonsResponse =
  PaginatedResponse<Season>;

export type SeasonDialogMode =
  | "create"
  | "edit";

export interface SeasonDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: SeasonDialogMode;
  season?: Season;
}

export interface SeasonFormProps {
  mode: SeasonDialogMode;
  season?: Season;
  formId: string;
  errorMessage?: string | null;
  onSubmit: (
    values: SeasonFormValues
  ) => Promise<void>;
}

export interface SeasonActionsProps {
  season: Season;
}
