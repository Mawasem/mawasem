import { useTranslation } from "react-i18next";

import { DeleteEntityDialog } from "@/components/entity-dialog/delete-entity-dialog";

import { useUnblockCustomer } from "../hooks/use-unblock-customer";
import type { UnblockCustomerDialogProps } from "../types";

export function UnblockCustomerDialog({
  customer,
  open,
  onOpenChange,
}: UnblockCustomerDialogProps) {
  const { t, i18n } = useTranslation();

  const unblockCustomerMutation =
    useUnblockCustomer();

  const errorMessage =
    unblockCustomerMutation.error instanceof Error
      ? unblockCustomerMutation.error.message
      : null;

  const handleUnblock = async () => {
    try {
      await unblockCustomerMutation.unblockCustomerAsync(
        customer.id
      );

      onOpenChange(false);
    } catch {
      // Keep dialog open and show mutation error.
    }
  };

  return (
    <DeleteEntityDialog
      open={open}
      onOpenChange={onOpenChange}
      title={t("customers.unblockDialog.title")}
      description={t("customers.unblockDialog.description")}
      entityName={
        i18n.resolvedLanguage === "ar"
          ? customer.fullNameAr
          : customer.fullNameEn
      }
      isDeleting={unblockCustomerMutation.isLoading}
      errorMessage={errorMessage}
      confirmLabel={t("customers.actions.unblock")}
      deletingLabel={t("customers.actions.unblocking")}
      cancelLabel={t("common.cancel")}
      onConfirm={handleUnblock}
    />
  );
}
