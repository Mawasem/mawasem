import { useTranslation } from "react-i18next";

import { EntityDialog } from "@/components/entity-dialog/entity-dialog";
import { EntityDialogFooter } from "@/components/entity-dialog/entity-dialog-footer";

import { getDeliveryAreaErrorMessage } from "../get-delivery-area-error-message";
import { useCreateDeliveryArea } from "../hooks/use-create-delivery-area";
import { useUpdateDeliveryArea } from "../hooks/use-update-delivery-area";
import type { DeliveryAreaFormValues } from "../schema/delivery-area-form-schema";
import type {
  DeliveryArea,
  DeliveryAreaDialogMode,
  DeliveryAreaDialogProps,
} from "../types";
import { DeliveryAreaForm } from "./delivery-area-form";

export function DeliveryAreaDialog({
  open,
  onOpenChange,
  mode,
  deliveryArea,
}: DeliveryAreaDialogProps) {
  const { t } = useTranslation();

  const isEditMode = mode === "edit";

  return (
    <EntityDialog
      open={open}
      onOpenChange={onOpenChange}
      title={
        isEditMode
          ? t("deliveryAreas.dialog.editTitle")
          : t("deliveryAreas.dialog.createTitle")
      }
      description={
        isEditMode
          ? t("deliveryAreas.dialog.editDescription")
          : t("deliveryAreas.dialog.createDescription")
      }
    >
      {open ? (
        <DeliveryAreaDialogContent
          key={`${mode}-${deliveryArea?.id ?? "new"}`}
          mode={mode}
          deliveryArea={deliveryArea}
          onClose={() => onOpenChange(false)}
        />
      ) : null}
    </EntityDialog>
  );
}

interface DeliveryAreaDialogContentProps {
  mode: DeliveryAreaDialogMode;
  deliveryArea?: DeliveryArea;
  onClose: () => void;
}

function DeliveryAreaDialogContent({
  mode,
  deliveryArea,
  onClose,
}: DeliveryAreaDialogContentProps) {
  const { t } = useTranslation();

  const createDeliveryAreaMutation = useCreateDeliveryArea();
  const updateDeliveryAreaMutation = useUpdateDeliveryArea();

  const isEditMode = mode === "edit";
  const formId = `delivery-area-form-${mode}`;
  const isSubmitting =
    createDeliveryAreaMutation.isLoading ||
    updateDeliveryAreaMutation.isLoading;

  const mutationError =
    createDeliveryAreaMutation.error ?? updateDeliveryAreaMutation.error;

  const errorMessage = mutationError
    ? getDeliveryAreaErrorMessage(mutationError, t)
    : null;

  const handleSubmit = async (values: DeliveryAreaFormValues) => {
    try {
      if (isEditMode && deliveryArea) {
        await updateDeliveryAreaMutation.updateDeliveryAreaAsync({
          deliveryAreaId: deliveryArea.id,
          data: {
            nameAr: values.nameAr,
            nameEn: values.nameEn,
            deliveryFee: values.isFreeDelivery ? 0 : values.deliveryFee,
            isFreeDelivery: values.isFreeDelivery,
            isActive: values.isActive,
          },
        });
      } else {
        await createDeliveryAreaMutation.createDeliveryAreaAsync({
          nameAr: values.nameAr,
          nameEn: values.nameEn,
          deliveryFee: values.isFreeDelivery ? 0 : values.deliveryFee,
          isFreeDelivery: values.isFreeDelivery,
          isActive: values.isActive,
          status: values.status,
        });
      }

      onClose();
    } catch {
      // Keep the dialog open and display the backend error.
    }
  };

  return (
    <div className="space-y-5">
      <DeliveryAreaForm
        mode={mode}
        deliveryArea={deliveryArea}
        formId={formId}
        errorMessage={errorMessage}
        onSubmit={handleSubmit}
      />

      <EntityDialogFooter
        mode={mode}
        formId={formId}
        isLoading={isSubmitting}
        onCancel={onClose}
        cancelLabel={t("common.cancel")}
        createLabel={t("deliveryAreas.actions.create")}
        createLoadingLabel={t("common.creating")}
        editLabel={t("common.saveChanges")}
        editLoadingLabel={t("common.saving")}
      />
    </div>
  );
}
