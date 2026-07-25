import type { EntityDialogMode } from "@/components/entity-dialog/types";

import type { CollectionFormValues } from "../schema/collection-form-schema";
import type { Collection } from "../types";

export type CollectionDialogMode =
  EntityDialogMode;

export interface CollectionDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: CollectionDialogMode;
  collection?: Collection;
}

export interface CollectionFormProps {
  mode: CollectionDialogMode;
  collection?: Collection;
  formId: string;
  errorMessage?: string | null;
  onSubmit: (
    values: CollectionFormValues
  ) => Promise<void>;
}

export interface CollectionActionsProps {
  collection: Collection;
}
