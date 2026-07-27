import { useTranslation } from "react-i18next";

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

import { getDeliveryAreaErrorMessage } from "../get-delivery-area-error-message";
import { useDeleteDeliveryArea } from "../hooks/use-delete-delivery-area";
import type { DeliveryAreaMutationDialogProps } from "../types";

export function DeliveryAreaDeleteDialog({
  deliveryArea,
  open,
  onOpenChange,
}: DeliveryAreaMutationDialogProps) {
  const { t, i18n } = useTranslation();
  const deleteMutation = useDeleteDeliveryArea();

  const entityName =
    i18n.resolvedLanguage === "ar"
      ? deliveryArea.nameAr
      : deliveryArea.nameEn;

  const isDeletionBlocked = deliveryArea.activeAddressCount > 0;
  const errorMessage = deleteMutation.error
    ? getDeliveryAreaErrorMessage(deleteMutation.error, t)
    : null;

  const handleConfirm = async () => {
    if (isDeletionBlocked) {
      return;
    }

    try {
      await deleteMutation.deleteDeliveryAreaAsync(deliveryArea.id);
      onOpenChange(false);
    } catch {
      // Keep the dialog open and display the backend error.
    }
  };

  const handleOpenChange = (nextOpen: boolean) => {
    if (!deleteMutation.isLoading) {
      onOpenChange(nextOpen);
    }
  };

  return (
    <AlertDialog open={open} onOpenChange={handleOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>
            {t("deliveryAreas.deleteDialog.title")}
          </AlertDialogTitle>

          <AlertDialogDescription>
            {t("deliveryAreas.deleteDialog.description")}
          </AlertDialogDescription>

          <p className="text-sm font-medium">{entityName}</p>

          {isDeletionBlocked ? (
            <p className="rounded-xl bg-destructive/10 p-3 text-sm text-destructive">
              {t("deliveryAreas.deleteDialog.blocked", {
                count: deliveryArea.activeAddressCount,
              })}
            </p>
          ) : null}

          {errorMessage ? (
            <p className="text-sm text-destructive">{errorMessage}</p>
          ) : null}
        </AlertDialogHeader>

        <AlertDialogFooter>
          <AlertDialogCancel asChild>
            <Button variant="outline" disabled={deleteMutation.isLoading}>
              {t("common.cancel")}
            </Button>
          </AlertDialogCancel>

          {!isDeletionBlocked ? (
            <Button
              variant="destructive"
              onClick={() => void handleConfirm()}
              disabled={deleteMutation.isLoading}
            >
              {deleteMutation.isLoading
                ? t("common.deleting")
                : t("common.delete")}
            </Button>
          ) : null}
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
