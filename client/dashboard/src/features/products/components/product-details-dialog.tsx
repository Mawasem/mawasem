import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Separator } from "@/components/ui/separator";

import { getProductErrorMessage } from "../get-product-error-message";
import { useProduct } from "../hooks/use-product";
import { formatProductPrice } from "../product-utils";
import type { ProductDialogEntityProps, ProductReference } from "../types";
import { ProductStatusBadges } from "./product-status-badges";

interface DetailItemProps {
  label: string;
  value: ReactNode;
}

function DetailItem({ label, value }: DetailItemProps) {
  return (
    <div className="space-y-1">
      <p className="text-sm text-muted-foreground">{label}</p>
      <div className="text-sm font-medium">{value}</div>
    </div>
  );
}

export function ProductDetailsDialog({
  product,
  open,
  onOpenChange,
}: ProductDialogEntityProps) {
  const { t, i18n } = useTranslation();
  const { productData, isLoading, error } = useProduct(product.id, open);
  const language = i18n.resolvedLanguage ?? "en";

  const names = (items: ProductReference[]) =>
    items.length > 0 ? (
      <div className="flex flex-wrap gap-2">
        {items.map((item) => (
          <Badge key={item.id} variant="outline">
            {language === "ar" ? item.nameAr : item.nameEn}
          </Badge>
        ))}
      </div>
    ) : (
      t("common.notAvailable")
    );

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] max-w-5xl overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{t("products.details.title")}</DialogTitle>
          <DialogDescription>
            {t("products.details.description")}
          </DialogDescription>
        </DialogHeader>

        {isLoading ? (
          <p className="text-sm text-muted-foreground">
            {t("products.details.loading")}
          </p>
        ) : error ? (
          <p className="text-sm text-destructive">
            {getProductErrorMessage(error, t)}
          </p>
        ) : productData ? (
          <div className="space-y-6">
            <div className="flex flex-wrap items-start justify-between gap-4 rounded-2xl border p-4">
              <div>
                <h3 className="text-lg font-semibold">
                  {language === "ar" ? productData.nameAr : productData.nameEn}
                </h3>
                <p className="text-sm text-muted-foreground">
                  {productData.slug}
                </p>
              </div>
              <ProductStatusBadges
                isPublished={productData.isPublished}
                isFeatured={productData.isFeatured}
                isDeleted={productData.isDeleted}
              />
            </div>

            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
              <DetailItem
                label={t("products.details.originalPrice")}
                value={formatProductPrice(productData.originalPrice, language)}
              />
              <DetailItem
                label={t("products.details.currentPrice")}
                value={formatProductPrice(productData.currentPrice, language)}
              />
              <DetailItem
                label={t("products.details.variants")}
                value={productData.variantCount}
              />
              <DetailItem
                label={t("products.details.stock")}
                value={productData.totalStock}
              />
            </div>

            <Separator />

            <div className="grid gap-5 md:grid-cols-2">
              <DetailItem
                label={t("products.details.nameAr")}
                value={productData.nameAr}
              />
              <DetailItem
                label={t("products.details.nameEn")}
                value={productData.nameEn}
              />
              <DetailItem
                label={t("products.details.descriptionAr")}
                value={<p className="whitespace-pre-wrap">{productData.descriptionAr}</p>}
              />
              <DetailItem
                label={t("products.details.descriptionEn")}
                value={<p className="whitespace-pre-wrap">{productData.descriptionEn}</p>}
              />
            </div>

            <Separator />

            <div className="grid gap-5 md:grid-cols-2">
              <DetailItem
                label={t("products.details.brand")}
                value={language === "ar" ? productData.brand.nameAr : productData.brand.nameEn}
              />
              <DetailItem
                label={t("products.details.season")}
                value={language === "ar" ? productData.season.nameAr : productData.season.nameEn}
              />
              <DetailItem
                label={t("products.details.categories")}
                value={names(productData.categories)}
              />
              <DetailItem
                label={t("products.details.collections")}
                value={names(productData.collections)}
              />
              <DetailItem
                label={t("products.details.grades")}
                value={names(productData.grades)}
              />
              <DetailItem
                label={t("products.details.tags")}
                value={names(productData.tags)}
              />
            </div>

            <Separator />

            <div className="space-y-3">
              <h3 className="font-semibold">
                {t("products.details.specifications")}
              </h3>
              {productData.specifications.length === 0 ? (
                <p className="text-sm text-muted-foreground">
                  {t("products.details.noSpecifications")}
                </p>
              ) : (
                <div className="grid gap-3 md:grid-cols-2">
                  {productData.specifications.map((specification) => (
                    <div key={specification.id} className="rounded-2xl border p-4">
                      <p className="font-medium">
                        {language === "ar"
                          ? specification.nameAr
                          : specification.nameEn}
                      </p>
                      <p className="mt-1 text-sm text-muted-foreground">
                        {language === "ar"
                          ? specification.valueAr
                          : specification.valueEn}
                      </p>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        ) : null}
      </DialogContent>
    </Dialog>
  );
}
