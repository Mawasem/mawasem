import {
  AlertDialog,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";

import type { EntityMutationDialogProps } from "./types";

export function EntityDeleteDialog<
  TEntityId = number,
>({
  open,
  onOpenChange,
  title,
  description,
  entityName,
  confirmLabel = "Delete",
  loadingLabel = "Deleting...",
  cancelLabel = "Cancel",
  mutation,
  entityId,
}: EntityMutationDialogProps<TEntityId>) {
  const isMutating =
    mutation.isPending ??
    mutation.isLoading ??
    false;

  const errorMessage =
    mutation.error instanceof Error
      ? mutation.error.message
      : null;

  const handleConfirm = async () => {
    try {
      await mutation.mutateAsync(entityId);
      onOpenChange(false);
    } catch {
      // Keep dialog open and show mutation error.
    }
  };

  return (
    <AlertDialog
      open={open}
      onOpenChange={onOpenChange}
    >
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>{title}</AlertDialogTitle>

          <AlertDialogDescription className="whitespace-pre-line">
            {description}
          </AlertDialogDescription>

          {entityName ? (
            <p className="text-sm font-medium">
              {entityName}
            </p>
          ) : null}

          {errorMessage ? (
            <p className="text-sm text-destructive">
              {errorMessage}
            </p>
          ) : null}
        </AlertDialogHeader>

        <AlertDialogFooter>
          <AlertDialogCancel asChild>
            <Button
              variant="outline"
              disabled={isMutating}
            >
              {cancelLabel}
            </Button>
          </AlertDialogCancel>

          <Button
            variant="destructive"
            onClick={() => {
              void handleConfirm();
            }}
            disabled={isMutating}
          >
            {isMutating
              ? loadingLabel
              : confirmLabel}
          </Button>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
