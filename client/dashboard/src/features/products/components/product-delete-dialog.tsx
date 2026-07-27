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

import { getProductErrorMessage } from "../get-product-error-message";
import { useDeleteProduct } from "../hooks/use-delete-product";
import type { ProductDialogEntityProps } from "../types";

export function ProductDeleteDialog({
  product,
  open,
  onOpenChange,
}: ProductDialogEntityProps) {
  const { t, i18n } = useTranslation();
  const mutation = useDeleteProduct();
  const name = i18n.resolvedLanguage === "ar" ? product.nameAr : product.nameEn;

  const handleConfirm = async () => {
    try {
      await mutation.deleteProductAsync(product.id);
      onOpenChange(false);
    } catch {
      // Keep open and display backend error.
    }
  };

  return (
    <AlertDialog
      open={open}
      onOpenChange={(nextOpen) => {
        if (!mutation.isLoading) onOpenChange(nextOpen);
      }}
    >
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>{t("products.deleteDialog.title")}</AlertDialogTitle>
          <AlertDialogDescription>
            {t("products.deleteDialog.description")}
          </AlertDialogDescription>
          <p className="text-sm font-medium">{name}</p>
          {mutation.error ? (
            <p className="text-sm text-destructive">
              {getProductErrorMessage(mutation.error, t)}
            </p>
          ) : null}
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel asChild>
            <Button variant="outline" disabled={mutation.isLoading}>
              {t("common.cancel")}
            </Button>
          </AlertDialogCancel>
          <Button
            variant="destructive"
            onClick={() => void handleConfirm()}
            disabled={mutation.isLoading}
          >
            {mutation.isLoading ? t("common.deleting") : t("common.delete")}
          </Button>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
