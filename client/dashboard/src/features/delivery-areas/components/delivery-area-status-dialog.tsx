import { useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";

import { EntityDialog } from "@/components/entity-dialog/entity-dialog";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";

import { getDeliveryAreaErrorMessage } from "../get-delivery-area-error-message";
import { useUpdateDeliveryAreaStatus } from "../hooks/use-update-delivery-area-status";
import {
  DeliveryAreaStatus,
  type DeliveryArea,
  type DeliveryAreaStatusDialogProps,
} from "../types";

const selectClassName =
  "h-9 w-full rounded-4xl border border-input bg-input/30 px-3 text-sm outline-none transition-colors focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50";

export function DeliveryAreaStatusDialog({
  deliveryArea,
  open,
  onOpenChange,
}: DeliveryAreaStatusDialogProps) {
  const { t, i18n } = useTranslation();

  const deliveryAreaName =
    i18n.resolvedLanguage === "ar"
      ? deliveryArea.nameAr
      : deliveryArea.nameEn;

  return (
    <EntityDialog
      open={open}
      onOpenChange={onOpenChange}
      title={t("deliveryAreas.statusDialog.title")}
      description={t("deliveryAreas.statusDialog.description", {
        name: deliveryAreaName,
      })}
    >
      {open ? (
        <DeliveryAreaStatusDialogContent
          key={`${deliveryArea.id}-${deliveryArea.status}`}
          deliveryArea={deliveryArea}
          onClose={() => onOpenChange(false)}
        />
      ) : null}
    </EntityDialog>
  );
}

interface DeliveryAreaStatusDialogContentProps {
  deliveryArea: DeliveryArea;
  onClose: () => void;
}

function DeliveryAreaStatusDialogContent({
  deliveryArea,
  onClose,
}: DeliveryAreaStatusDialogContentProps) {
  const { t } = useTranslation();
  const [status, setStatus] = useState<DeliveryAreaStatus>(
    deliveryArea.status
  );

  const updateStatusMutation = useUpdateDeliveryAreaStatus();

  const errorMessage = updateStatusMutation.error
    ? getDeliveryAreaErrorMessage(updateStatusMutation.error, t)
    : null;

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    try {
      await updateStatusMutation.updateDeliveryAreaStatusAsync({
        deliveryAreaId: deliveryArea.id,
        data: { status },
      });

      onClose();
    } catch {
      // Keep the dialog open and display the backend error.
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-5">
      <div className="space-y-2">
        <Label htmlFor={`delivery-area-status-${deliveryArea.id}`}>
          {t("deliveryAreas.statusDialog.statusLabel")}
        </Label>

        <select
          id={`delivery-area-status-${deliveryArea.id}`}
          className={selectClassName}
          value={status}
          onChange={(event) =>
            setStatus(Number(event.target.value) as DeliveryAreaStatus)
          }
          disabled={updateStatusMutation.isLoading}
        >
          <option value={DeliveryAreaStatus.Pending}>
            {t("deliveryAreas.status.pending")}
          </option>
          <option value={DeliveryAreaStatus.Confirmed}>
            {t("deliveryAreas.status.confirmed")}
          </option>
          <option value={DeliveryAreaStatus.Restricted}>
            {t("deliveryAreas.status.restricted")}
          </option>
        </select>

        <p className="text-sm text-muted-foreground">
          {t("deliveryAreas.statusDialog.hint")}
        </p>
      </div>

      {errorMessage ? (
        <p className="text-sm text-destructive">{errorMessage}</p>
      ) : null}

      <div className="flex justify-end gap-2">
        <Button
          type="button"
          variant="outline"
          onClick={onClose}
          disabled={updateStatusMutation.isLoading}
        >
          {t("common.cancel")}
        </Button>

        <Button
          type="submit"
          disabled={
            updateStatusMutation.isLoading || status === deliveryArea.status
          }
        >
          {updateStatusMutation.isLoading
            ? t("common.saving")
            : t("common.saveChanges")}
        </Button>
      </div>
    </form>
  );
}
