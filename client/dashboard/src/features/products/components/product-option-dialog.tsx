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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { getProductErrorMessage } from "../get-product-error-message";
import { useCreateProductOption } from "../hooks/use-create-product-option";
import { useUpdateProductOption } from "../hooks/use-update-product-option";
import {
  ProductOptionType,
  type ProductOption,
  type ProductOptionType as ProductOptionTypeValue,
} from "../types";

interface ProductOptionDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  option?: ProductOption;
}

export function ProductOptionDialog({
  open,
  onOpenChange,
  option,
}: ProductOptionDialogProps) {
  const { t } = useTranslation();
  const isEditMode = Boolean(option);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {isEditMode
              ? t("products.options.editTitle")
              : t("products.options.createTitle")}
          </DialogTitle>
          <DialogDescription>
            {t("products.options.dialogDescription")}
          </DialogDescription>
        </DialogHeader>
        {open ? (
          <ProductOptionDialogContent
            key={option?.id ?? "new"}
            option={option}
            onClose={() => onOpenChange(false)}
          />
        ) : null}
      </DialogContent>
    </Dialog>
  );
}

interface ProductOptionDialogContentProps {
  option?: ProductOption;
  onClose: () => void;
}

function ProductOptionDialogContent({
  option,
  onClose,
}: ProductOptionDialogContentProps) {
  const { t } = useTranslation();
  const [nameAr, setNameAr] = useState(option?.nameAr ?? "");
  const [nameEn, setNameEn] = useState(option?.nameEn ?? "");
  const [type, setType] = useState<ProductOptionTypeValue>(
    option?.type ?? ProductOptionType.Standard
  );
  const createMutation = useCreateProductOption();
  const updateMutation = useUpdateProductOption();
  const isSubmitting = createMutation.isLoading || updateMutation.isLoading;
  const error = createMutation.error ?? updateMutation.error;
  const isValid = nameAr.trim().length > 0 && nameEn.trim().length > 0;

  const handleSave = async () => {
    if (!isValid) return;

    try {
      if (option) {
        await updateMutation.updateProductOptionAsync({
          optionId: option.id,
          data: { nameAr: nameAr.trim(), nameEn: nameEn.trim() },
        });
      } else {
        await createMutation.createProductOptionAsync({
          nameAr: nameAr.trim(),
          nameEn: nameEn.trim(),
          type,
        });
      }
      onClose();
    } catch {
      // Keep dialog open and show backend error.
    }
  };

  return (
    <div className="space-y-5">
      <div className="grid gap-4 md:grid-cols-2">
        <div className="space-y-2">
          <Label htmlFor="product-option-name-ar">
            {t("products.options.nameAr")}
          </Label>
          <Input
            id="product-option-name-ar"
            dir="rtl"
            value={nameAr}
            onChange={(event) => setNameAr(event.target.value)}
            disabled={isSubmitting}
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="product-option-name-en">
            {t("products.options.nameEn")}
          </Label>
          <Input
            id="product-option-name-en"
            dir="ltr"
            value={nameEn}
            onChange={(event) => setNameEn(event.target.value)}
            disabled={isSubmitting}
          />
        </div>
      </div>

      <div className="space-y-2">
        <Label>{t("products.options.type")}</Label>
        <Select
          value={String(type)}
          onValueChange={(value) =>
            setType(Number(value) as ProductOptionTypeValue)
          }
          disabled={Boolean(option) || isSubmitting}
        >
          <SelectTrigger className="w-full">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={String(ProductOptionType.Standard)}>
              {t("products.options.standard")}
            </SelectItem>
            <SelectItem value={String(ProductOptionType.Color)}>
              {t("products.options.color")}
            </SelectItem>
          </SelectContent>
        </Select>
        {option ? (
          <p className="text-xs text-muted-foreground">
            {t("products.options.typeImmutable")}
          </p>
        ) : null}
      </div>

      {!isValid ? (
        <p className="text-sm text-destructive">
          {t("products.validation.requiredNames")}
        </p>
      ) : null}
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
