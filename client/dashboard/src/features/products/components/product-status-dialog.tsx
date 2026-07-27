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
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";

import { getProductErrorMessage } from "../get-product-error-message";
import { useUpdateProductStatus } from "../hooks/use-update-product-status";
import type { ProductDialogEntityProps } from "../types";

export function ProductStatusDialog(props: ProductDialogEntityProps) {
  return (
    <Dialog open={props.open} onOpenChange={props.onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{useTranslation().t("products.statusDialog.title")}</DialogTitle>
          <DialogDescription>
            {useTranslation().t("products.statusDialog.description")}
          </DialogDescription>
        </DialogHeader>
        {props.open ? (
          <ProductStatusDialogContent
            key={props.product.id}
            product={props.product}
            onClose={() => props.onOpenChange(false)}
          />
        ) : null}
      </DialogContent>
    </Dialog>
  );
}

interface ProductStatusDialogContentProps {
  product: ProductDialogEntityProps["product"];
  onClose: () => void;
}

function ProductStatusDialogContent({
  product,
  onClose,
}: ProductStatusDialogContentProps) {
  const { t } = useTranslation();
  const [isPublished, setIsPublished] = useState(product.isPublished);
  const [isFeatured, setIsFeatured] = useState(product.isFeatured);
  const mutation = useUpdateProductStatus();

  const handlePublishedChange = (checked: boolean) => {
    setIsPublished(checked);
    if (!checked) {
      setIsFeatured(false);
    }
  };

  const handleFeaturedChange = (checked: boolean) => {
    setIsFeatured(checked);
    if (checked) {
      setIsPublished(true);
    }
  };

  const handleSave = async () => {
    try {
      await mutation.updateProductStatusAsync({
        productId: product.id,
        data: { isPublished, isFeatured },
      });
      onClose();
    } catch {
      // Keep open and show backend validation, including publication prerequisites.
    }
  };

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between gap-4 rounded-2xl border p-4">
        <div>
          <Label htmlFor={`product-published-${product.id}`}>
            {t("products.statusDialog.published")}
          </Label>
          <p className="text-sm text-muted-foreground">
            {t("products.statusDialog.publishedDescription")}
          </p>
        </div>
        <Switch
          id={`product-published-${product.id}`}
          checked={isPublished}
          onCheckedChange={handlePublishedChange}
          disabled={mutation.isLoading}
        />
      </div>

      <div className="flex items-center justify-between gap-4 rounded-2xl border p-4">
        <div>
          <Label htmlFor={`product-featured-${product.id}`}>
            {t("products.statusDialog.featured")}
          </Label>
          <p className="text-sm text-muted-foreground">
            {t("products.statusDialog.featuredDescription")}
          </p>
        </div>
        <Switch
          id={`product-featured-${product.id}`}
          checked={isFeatured}
          onCheckedChange={handleFeaturedChange}
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
        <Button onClick={() => void handleSave()} disabled={mutation.isLoading}>
          {mutation.isLoading ? t("common.saving") : t("common.save")}
        </Button>
      </DialogFooter>
    </div>
  );
}
