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
import type { DeleteEntityDialogProps } from "./types";

export function DeleteEntityDialog({
  open,
  onOpenChange,
  title,
  description,
  entityName,
  isDeleting = false,
  errorMessage,
  confirmLabel = "Delete",
  cancelLabel = "Cancel",
  onConfirm,
}: DeleteEntityDialogProps) {
  const handleConfirm = async () => {
    await onConfirm();
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
              disabled={isDeleting}
            >
              {cancelLabel}
            </Button>
          </AlertDialogCancel>

          <Button
            variant="destructive"
            onClick={() => {
              void handleConfirm();
            }}
            disabled={isDeleting}
          >
            {isDeleting
              ? "Deleting..."
              : confirmLabel}
          </Button>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
