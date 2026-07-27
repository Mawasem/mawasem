import { useMemo, useState } from "react";
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
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { getProductErrorMessage } from "../get-product-error-message";
import { useCreateProductVariant } from "../hooks/use-create-product-variant";
import { useProductOptions } from "../hooks/use-product-options";
import type { ProductVariant } from "../types";

interface CreateProductVariantDialogProps {
  productId: number;
  variants: ProductVariant[];
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function CreateProductVariantDialog({
  productId,
  variants,
  open,
  onOpenChange,
}: CreateProductVariantDialogProps) {
  const { t } = useTranslation();

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[85vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{t("products.variants.createTitle")}</DialogTitle>
          <DialogDescription>
            {t("products.variants.createDescription")}
          </DialogDescription>
        </DialogHeader>
        {open ? (
          <CreateProductVariantDialogContent
            key={productId}
            productId={productId}
            variants={variants}
            onClose={() => onOpenChange(false)}
          />
        ) : null}
      </DialogContent>
    </Dialog>
  );
}

interface CreateProductVariantDialogContentProps {
  productId: number;
  variants: ProductVariant[];
  onClose: () => void;
}

function CreateProductVariantDialogContent({
  productId,
  variants,
  onClose,
}: CreateProductVariantDialogContentProps) {
  const { t, i18n } = useTranslation();
  const { productOptionsData, isLoading, error } = useProductOptions();
  const mutation = useCreateProductVariant();

  const requiredOptionIds = useMemo(() => {
    const structuralVariant = variants.find(
      (variant) => variant.isAvailable && variant.options.length > 0
    );
    return structuralVariant?.options.map((option) => option.optionId) ?? [];
  }, [variants]);

  const visibleOptions = useMemo(() => {
    const options = productOptionsData ?? [];
    if (requiredOptionIds.length === 0) return options;
    return options.filter((option) => requiredOptionIds.includes(option.id));
  }, [productOptionsData, requiredOptionIds]);

  const [selectedValueIds, setSelectedValueIds] = useState<
    Record<number, number | undefined>
  >({});

  const isRequiredStructure = requiredOptionIds.length > 0;
  const isSelectionValid =
    !isRequiredStructure ||
    requiredOptionIds.every((optionId) => selectedValueIds[optionId] !== undefined);

  const handleCreate = async () => {
    if (!isSelectionValid) return;
    const optionValueIds = visibleOptions
      .map((option) => selectedValueIds[option.id])
      .filter((valueId): valueId is number => valueId !== undefined);

    try {
      await mutation.createProductVariantAsync({
        productId,
        data: { optionValueIds },
      });
      onClose();
    } catch {
      // Keep open and show backend combination/structure errors.
    }
  };

  const displayName = (item: { nameAr: string; nameEn: string }) =>
    i18n.resolvedLanguage === "ar" ? item.nameAr : item.nameEn;

  return (
    <div className="space-y-5">
      {isLoading ? (
        <p className="text-sm text-muted-foreground">{t("common.loading")}</p>
      ) : error ? (
        <p className="text-sm text-destructive">
          {getProductErrorMessage(error, t)}
        </p>
      ) : visibleOptions.length === 0 ? (
        <div className="rounded-2xl border border-dashed p-6 text-sm text-muted-foreground">
          {t("products.variants.defaultVariantHint")}
        </div>
      ) : (
        <div className="space-y-4">
          {visibleOptions.map((option) => (
            <div key={option.id} className="space-y-2">
              <Label>{displayName(option)}</Label>
              <Select
                value={
                  selectedValueIds[option.id] === undefined
                    ? "none"
                    : String(selectedValueIds[option.id])
                }
                onValueChange={(value) =>
                  setSelectedValueIds((current) => ({
                    ...current,
                    [option.id]: value === "none" ? undefined : Number(value),
                  }))
                }
                disabled={mutation.isLoading}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder={t("products.variants.selectValue")} />
                </SelectTrigger>
                <SelectContent>
                  {!isRequiredStructure ? (
                    <SelectItem value="none">
                      {t("products.variants.notUsed")}
                    </SelectItem>
                  ) : null}
                  {option.values.map((value) => (
                    <SelectItem key={value.id} value={String(value.id)}>
                      {i18n.resolvedLanguage === "ar"
                        ? value.valueAr
                        : value.valueEn}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          ))}
        </div>
      )}

      {isRequiredStructure ? (
        <p className="text-xs text-muted-foreground">
          {t("products.variants.structureLocked")}
        </p>
      ) : null}

      {mutation.error ? (
        <p className="text-sm text-destructive">
          {getProductErrorMessage(mutation.error, t)}
        </p>
      ) : null}

      <DialogFooter>
        <Button variant="outline" onClick={onClose} disabled={mutation.isLoading}>
          {t("common.cancel")}
        </Button>
        <Button
          onClick={() => void handleCreate()}
          disabled={isLoading || mutation.isLoading || !isSelectionValid}
        >
          {mutation.isLoading ? t("common.creating") : t("products.variants.create")}
        </Button>
      </DialogFooter>
    </div>
  );
}
