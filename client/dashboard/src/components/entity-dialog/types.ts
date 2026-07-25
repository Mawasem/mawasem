import type { ReactNode } from "react";

export type EntityDialogMode =
  | "create"
  | "edit";

export interface EntityDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  title: string;
  description: string;
  children: ReactNode;
}

export interface EntityDialogFooterProps {
  mode: EntityDialogMode;
  formId: string;
  isLoading?: boolean;
  onCancel: () => void;
  cancelLabel?: string;
  createLabel?: string;
  createLoadingLabel?: string;
  editLabel?: string;
  editLoadingLabel?: string;
}

export interface DeleteEntityDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  title: string;
  description: string;
  entityName?: string;
  isDeleting?: boolean;
  errorMessage?: string | null;
  confirmLabel?: string;
  deletingLabel?: string;
  cancelLabel?: string;
  onConfirm: () => Promise<void>;
}

export interface EntityMutation<
  TEntityId = number,
> {
  mutateAsync: (
    entityId: TEntityId
  ) => Promise<unknown>;
  isPending?: boolean;
  isLoading?: boolean;
  error?: unknown;
}

export interface EntityMutationDialogProps<
  TEntityId = number,
> {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  title: string;
  description: string;
  entityName?: string;
  confirmLabel?: string;
  loadingLabel?: string;
  cancelLabel?: string;
  mutation: EntityMutation<TEntityId>;
  entityId: TEntityId;
}
