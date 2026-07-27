import { useTranslation } from "react-i18next";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

import { getProductErrorMessage } from "../get-product-error-message";
import { useCreateProduct } from "../hooks/use-create-product";
import { useProduct } from "../hooks/use-product";
import { useUpdateProduct } from "../hooks/use-update-product";
import type { ProductFormValues } from "../schema/product-form-schema";
import type { ProductDialogProps, ProductPayload } from "../types";
import { ProductForm } from "./product-form";

export function ProductDialog({
  open,
  onOpenChange,
  mode,
  product,
}: ProductDialogProps) {
  const { t } = useTranslation();
  const isEditMode = mode === "edit";

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="flex max-h-[92vh] max-w-5xl flex-col overflow-hidden">
        <DialogHeader>
          <DialogTitle>
            {isEditMode
              ? t("products.dialog.editTitle")
              : t("products.dialog.createTitle")}
          </DialogTitle>
          <DialogDescription>
            {isEditMode
              ? t("products.dialog.editDescription")
              : t("products.dialog.createDescription")}
          </DialogDescription>
        </DialogHeader>

        {open ? (
          <ProductDialogContent
            key={`${mode}-${product?.id ?? "new"}`}
            mode={mode}
            productId={product?.id}
            onClose={() => onOpenChange(false)}
          />
        ) : null}
      </DialogContent>
    </Dialog>
  );
}

interface ProductDialogContentProps {
  mode: "create" | "edit";
  productId?: number;
  onClose: () => void;
}

function ProductDialogContent({
  mode,
  productId,
  onClose,
}: ProductDialogContentProps) {
  const { t } = useTranslation();
  const isEditMode = mode === "edit";
  const formId = `product-form-${mode}-${productId ?? "new"}`;

  const { productData, isLoading: isProductLoading, error: productError } =
    useProduct(productId ?? 0, isEditMode);
  const createMutation = useCreateProduct();
  const updateMutation = useUpdateProduct();

  const isSubmitting = createMutation.isLoading || updateMutation.isLoading;
  const mutationError = createMutation.error ?? updateMutation.error;
  const errorMessage = mutationError
    ? getProductErrorMessage(mutationError, t)
    : null;

  const handleSubmit = async (values: ProductFormValues) => {
    const payload: ProductPayload = {
      ...values,
      gradeIds: productData?.grades.map((item) => item.id) ?? [],
      tagIds: productData?.tags.map((item) => item.id) ?? [],
    };

    try {
      if (isEditMode && productId) {
        await updateMutation.updateProductAsync({
          productId,
          data: payload,
        });
      } else {
        await createMutation.createProductAsync(payload);
      }

      onClose();
    } catch {
      // Keep the dialog open and surface the backend problem details.
    }
  };

  if (isEditMode && isProductLoading) {
    return (
      <p className="py-8 text-sm text-muted-foreground">
        {t("products.details.loading")}
      </p>
    );
  }

  if (productError) {
    return (
      <p className="py-8 text-sm text-destructive">
        {getProductErrorMessage(productError, t)}
      </p>
    );
  }

  if (isEditMode && !productData) {
    return null;
  }

  return (
    <>
      <div className="min-h-0 flex-1 overflow-y-auto pe-1">
        <ProductForm
          mode={mode}
          product={productData}
          formId={formId}
          errorMessage={errorMessage}
          onSubmit={handleSubmit}
        />
      </div>

      <DialogFooter className="border-t pt-4">
        <Button
          type="button"
          variant="outline"
          onClick={onClose}
          disabled={isSubmitting}
        >
          {t("common.cancel")}
        </Button>
        <Button type="submit" form={formId} disabled={isSubmitting}>
          {isSubmitting
            ? isEditMode
              ? t("common.saving")
              : t("common.creating")
            : isEditMode
              ? t("common.saveChanges")
              : t("products.actions.create")}
        </Button>
      </DialogFooter>
    </>
  );
}
