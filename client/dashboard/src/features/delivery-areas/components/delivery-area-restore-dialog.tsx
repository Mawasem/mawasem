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
import { useRestoreDeliveryArea } from "../hooks/use-restore-delivery-area";
import type { DeliveryAreaMutationDialogProps } from "../types";

export function DeliveryAreaRestoreDialog({
  deliveryArea,
  open,
  onOpenChange,
}: DeliveryAreaMutationDialogProps) {
  const { t, i18n } = useTranslation();
  const restoreMutation = useRestoreDeliveryArea();

  const entityName =
    i18n.resolvedLanguage === "ar"
      ? deliveryArea.nameAr
      : deliveryArea.nameEn;

  const errorMessage = restoreMutation.error
    ? getDeliveryAreaErrorMessage(restoreMutation.error, t)
    : null;

  const handleConfirm = async () => {
    try {
      await restoreMutation.restoreDeliveryAreaAsync(deliveryArea.id);
      onOpenChange(false);
    } catch {
      // Keep the dialog open and display the backend error.
    }
  };

  const handleOpenChange = (nextOpen: boolean) => {
    if (!restoreMutation.isLoading) {
      onOpenChange(nextOpen);
    }
  };

  return (
    <AlertDialog open={open} onOpenChange={handleOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>
            {t("deliveryAreas.restoreDialog.title")}
          </AlertDialogTitle>

          <AlertDialogDescription>
            {t("deliveryAreas.restoreDialog.description")}
          </AlertDialogDescription>

          <p className="text-sm font-medium">{entityName}</p>

          {errorMessage ? (
            <p className="text-sm text-destructive">{errorMessage}</p>
          ) : null}
        </AlertDialogHeader>

        <AlertDialogFooter>
          <AlertDialogCancel asChild>
            <Button variant="outline" disabled={restoreMutation.isLoading}>
              {t("common.cancel")}
            </Button>
          </AlertDialogCancel>

          <Button
            onClick={() => void handleConfirm()}
            disabled={restoreMutation.isLoading}
          >
            {restoreMutation.isLoading
              ? t("common.restoring")
              : t("deliveryAreas.actions.restore")}
          </Button>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
