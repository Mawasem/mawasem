import { useState } from "react";
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
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import { getProductErrorMessage } from "../get-product-error-message";
import { useCreateProductOptionValue } from "../hooks/use-create-product-option-value";
import { useUpdateProductOptionValue } from "../hooks/use-update-product-option-value";
import type { ProductOption, ProductOptionValue } from "../types";

interface ProductOptionValueDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  option: ProductOption;
  value?: ProductOptionValue;
}

export function ProductOptionValueDialog({
  open,
  onOpenChange,
  option,
  value,
}: ProductOptionValueDialogProps) {
  const { t } = useTranslation();

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {value
              ? t("products.options.editValueTitle")
              : t("products.options.addValueTitle")}
          </DialogTitle>
          <DialogDescription>
            {t("products.options.valueDescription")}
          </DialogDescription>
        </DialogHeader>
        {open ? (
          <ProductOptionValueDialogContent
            key={value?.id ?? "new"}
            option={option}
            value={value}
            onClose={() => onOpenChange(false)}
          />
        ) : null}
      </DialogContent>
    </Dialog>
  );
}

interface ProductOptionValueDialogContentProps {
  option: ProductOption;
  value?: ProductOptionValue;
  onClose: () => void;
}

function ProductOptionValueDialogContent({
  option,
  value,
  onClose,
}: ProductOptionValueDialogContentProps) {
  const { t } = useTranslation();
  const [valueAr, setValueAr] = useState(value?.valueAr ?? "");
  const [valueEn, setValueEn] = useState(value?.valueEn ?? "");
  const createMutation = useCreateProductOptionValue();
  const updateMutation = useUpdateProductOptionValue();
  const isSubmitting = createMutation.isLoading || updateMutation.isLoading;
  const error = createMutation.error ?? updateMutation.error;
  const isValid = valueAr.trim().length > 0 && valueEn.trim().length > 0;

  const handleSave = async () => {
    if (!isValid) return;

    try {
      if (value) {
        await updateMutation.updateProductOptionValueAsync({
          optionId: option.id,
          valueId: value.id,
          data: { valueAr: valueAr.trim(), valueEn: valueEn.trim() },
        });
      } else {
        await createMutation.createProductOptionValueAsync({
          optionId: option.id,
          data: { valueAr: valueAr.trim(), valueEn: valueEn.trim() },
        });
      }
      onClose();
    } catch {
      // Keep open and show backend error.
    }
  };

  return (
    <div className="space-y-5">
      <div className="grid gap-4 md:grid-cols-2">
        <div className="space-y-2">
          <Label htmlFor="product-option-value-ar">
            {t("products.options.valueAr")}
          </Label>
          <Input
            id="product-option-value-ar"
            dir="rtl"
            value={valueAr}
            onChange={(event) => setValueAr(event.target.value)}
            disabled={isSubmitting}
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="product-option-value-en">
            {t("products.options.valueEn")}
          </Label>
          <Input
            id="product-option-value-en"
            dir="ltr"
            value={valueEn}
            onChange={(event) => setValueEn(event.target.value)}
            disabled={isSubmitting}
          />
        </div>
      </div>

      {error ? (
        <p className="text-sm text-destructive">
          {getProductErrorMessage(error, t)}
        </p>
      ) : null}

      <DialogFooter>
        <Button variant="outline" onClick={onClose} disabled={isSubmitting}>
          {t("common.cancel")}
        </Button>
        <Button
          onClick={() => void handleSave()}
          disabled={!isValid || isSubmitting}
        >
          {isSubmitting ? t("common.saving") : t("common.save")}
        </Button>
      </DialogFooter>
    </div>
  );
}
