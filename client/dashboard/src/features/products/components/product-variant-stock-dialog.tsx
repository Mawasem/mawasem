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
import { useUpdateProductVariantStock } from "../hooks/use-update-product-variant-stock";
import type { ProductVariant } from "../types";

interface ProductVariantStockDialogProps {
  productId: number;
  variant: ProductVariant;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function ProductVariantStockDialog({
  productId,
  variant,
  open,
  onOpenChange,
}: ProductVariantStockDialogProps) {
  const { t } = useTranslation();

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t("products.variants.stockTitle")}</DialogTitle>
          <DialogDescription>
            {t("products.variants.stockDescription", { sku: variant.sku })}
          </DialogDescription>
        </DialogHeader>
        {open ? (
          <ProductVariantStockDialogContent
            key={`${variant.id}-${variant.rowVersion}`}
            productId={productId}
            variant={variant}
            onClose={() => onOpenChange(false)}
          />
        ) : null}
      </DialogContent>
    </Dialog>
  );
}

interface ProductVariantStockDialogContentProps {
  productId: number;
  variant: ProductVariant;
  onClose: () => void;
}

function ProductVariantStockDialogContent({
  productId,
  variant,
  onClose,
}: ProductVariantStockDialogContentProps) {
  const { t } = useTranslation();
  const [stockQuantity, setStockQuantity] = useState(variant.stockQuantity);
  const mutation = useUpdateProductVariantStock();

  const handleSave = async () => {
    if (!Number.isInteger(stockQuantity) || stockQuantity < 0) return;

    try {
      await mutation.updateProductVariantStockAsync({
        productId,
        variantId: variant.id,
        data: {
          stockQuantity,
          rowVersion: variant.rowVersion,
        },
      });
      onClose();
    } catch {
      // Keep open and display concurrency/backend errors.
    }
  };

  return (
    <div className="space-y-5">
      <div className="space-y-2">
        <Label htmlFor={`variant-stock-${variant.id}`}>
          {t("products.variants.stockQuantity")}
        </Label>
        <Input
          id={`variant-stock-${variant.id}`}
          type="number"
          min="0"
          step="1"
          value={stockQuantity}
          onChange={(event) => setStockQuantity(event.target.valueAsNumber)}
          disabled={mutation.isLoading}
        />
      </div>

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
          onClick={() => void handleSave()}
          disabled={
            mutation.isLoading ||
            !Number.isInteger(stockQuantity) ||
            stockQuantity < 0
          }
        >
          {mutation.isLoading ? t("common.saving") : t("common.save")}
        </Button>
      </DialogFooter>
    </div>
  );
}
