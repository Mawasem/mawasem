import { Boxes, Plus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Switch } from "@/components/ui/switch";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

import { getProductErrorMessage } from "../get-product-error-message";
import { useProductVariants } from "../hooks/use-product-variants";
import { useUpdateProductVariantAvailability } from "../hooks/use-update-product-variant-availability";
import type { ProductDialogEntityProps, ProductVariant } from "../types";
import { CreateProductVariantDialog } from "./create-product-variant-dialog";
import { ProductVariantStockDialog } from "./product-variant-stock-dialog";

export function ProductVariantsDialog({
  product,
  open,
  onOpenChange,
}: ProductDialogEntityProps) {
  const { t, i18n } = useTranslation();
  const { productVariantsData, isLoading, error } = useProductVariants(
    product.id,
    open
  );
  const availabilityMutation = useUpdateProductVariantAvailability();
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [stockVariant, setStockVariant] = useState<ProductVariant | null>(null);
  const variants = productVariantsData ?? [];

  const handleAvailability = async (
    variant: ProductVariant,
    isAvailable: boolean
  ) => {
    try {
      await availabilityMutation.updateProductVariantAvailabilityAsync({
        productId: product.id,
        variantId: variant.id,
        data: { isAvailable },
      });
    } catch {
      // Mutation error is shown below the table.
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="flex max-h-[90vh] max-w-5xl flex-col overflow-hidden">
        <DialogHeader>
          <DialogTitle>{t("products.variants.title")}</DialogTitle>
          <DialogDescription>
            {t("products.variants.description")}
          </DialogDescription>
        </DialogHeader>

        <div className="flex items-center justify-between gap-3">
          <Badge variant="secondary">
            {t("products.variants.count", { count: variants.length })}
          </Badge>
          <Button onClick={() => setIsCreateOpen(true)} disabled={isLoading}>
            <Plus className="size-4" />
            {t("products.variants.create")}
          </Button>
        </div>

        <div className="min-h-0 flex-1 overflow-auto rounded-2xl border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>{t("products.variants.sku")}</TableHead>
                <TableHead>{t("products.variants.options")}</TableHead>
                <TableHead>{t("products.variants.stock")}</TableHead>
                <TableHead>{t("products.variants.available")}</TableHead>
                <TableHead>{t("products.table.headers.actions")}</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {isLoading ? (
                <TableRow>
                  <TableCell colSpan={5} className="h-24 text-center">
                    {t("common.loading")}
                  </TableCell>
                </TableRow>
              ) : variants.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={5} className="h-24 text-center">
                    <div className="flex flex-col items-center gap-2 text-muted-foreground">
                      <Boxes className="size-8" />
                      {t("products.variants.empty")}
                    </div>
                  </TableCell>
                </TableRow>
              ) : (
                variants.map((variant) => (
                  <TableRow key={variant.id}>
                    <TableCell className="font-mono text-xs">
                      {variant.sku}
                    </TableCell>
                    <TableCell>
                      <div className="flex min-w-48 flex-wrap gap-2">
                        {variant.options.length === 0 ? (
                          <Badge variant="outline">
                            {t("products.variants.defaultVariant")}
                          </Badge>
                        ) : (
                          variant.options.map((option) => (
                            <Badge key={option.optionId} variant="outline">
                              {i18n.resolvedLanguage === "ar"
                                ? `${option.optionNameAr}: ${option.valueAr}`
                                : `${option.optionNameEn}: ${option.valueEn}`}
                            </Badge>
                          ))
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge
                        variant={variant.stockQuantity > 0 ? "secondary" : "destructive"}
                      >
                        {variant.stockQuantity}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Switch
                        checked={variant.isAvailable}
                        onCheckedChange={(checked) =>
                          void handleAvailability(variant, checked)
                        }
                        disabled={availabilityMutation.isLoading}
                        aria-label={t("products.variants.available")}
                      />
                    </TableCell>
                    <TableCell>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setStockVariant(variant)}
                      >
                        {t("products.variants.updateStock")}
                      </Button>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>

        {error || availabilityMutation.error ? (
          <p className="text-sm text-destructive">
            {getProductErrorMessage(error ?? availabilityMutation.error, t)}
          </p>
        ) : null}

        <CreateProductVariantDialog
          productId={product.id}
          variants={variants}
          open={isCreateOpen}
          onOpenChange={setIsCreateOpen}
        />
        {stockVariant ? (
          <ProductVariantStockDialog
            productId={product.id}
            variant={stockVariant}
            open
            onOpenChange={(nextOpen) => {
              if (!nextOpen) setStockVariant(null);
            }}
          />
        ) : null}
      </DialogContent>
    </Dialog>
  );
}
