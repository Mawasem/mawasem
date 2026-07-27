import type { ColumnDef } from "@tanstack/react-table";
import { useMemo } from "react";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";

import { formatProductPrice } from "../product-utils";
import type { ProductListItem } from "../types";
import { ProductActions } from "./product-actions";
import { ProductStatusBadges } from "./product-status-badges";

export function useProductColumns() {
  const { t, i18n } = useTranslation();

  return useMemo<ColumnDef<ProductListItem>[]>(
    () => [
      {
        id: "name",
        header: t("products.table.headers.product"),
        cell: ({ row }) => (
          <div className="min-w-48 space-y-1">
            <p className="font-medium">
              {i18n.resolvedLanguage === "ar"
                ? row.original.nameAr
                : row.original.nameEn}
            </p>
            <p className="text-xs text-muted-foreground">
              {row.original.slug}
            </p>
          </div>
        ),
      },
      {
        id: "brandSeason",
        header: t("products.table.headers.classification"),
        cell: ({ row }) => (
          <div className="min-w-36 space-y-1 text-sm">
            <p>
              {i18n.resolvedLanguage === "ar"
                ? row.original.brand.nameAr
                : row.original.brand.nameEn}
            </p>
            <p className="text-xs text-muted-foreground">
              {i18n.resolvedLanguage === "ar"
                ? row.original.season.nameAr
                : row.original.season.nameEn}
            </p>
          </div>
        ),
      },
      {
        id: "price",
        header: t("products.table.headers.price"),
        cell: ({ row }) => (
          <div className="whitespace-nowrap">
            <p className="font-medium">
              {formatProductPrice(
                row.original.currentPrice,
                i18n.resolvedLanguage ?? "en"
              )}
            </p>
            {row.original.currentPrice < row.original.originalPrice ? (
              <p className="text-xs text-muted-foreground line-through">
                {formatProductPrice(
                  row.original.originalPrice,
                  i18n.resolvedLanguage ?? "en"
                )}
              </p>
            ) : null}
          </div>
        ),
      },
      {
        id: "inventory",
        header: t("products.table.headers.inventory"),
        cell: ({ row }) => (
          <div className="flex flex-wrap gap-2">
            <Badge variant="outline">
              {t("products.table.variants", {
                count: row.original.variantCount,
              })}
            </Badge>
            <Badge
              variant={row.original.totalStock > 0 ? "secondary" : "destructive"}
            >
              {t("products.table.stock", { count: row.original.totalStock })}
            </Badge>
          </div>
        ),
      },
      {
        id: "status",
        header: t("products.table.headers.status"),
        cell: ({ row }) => (
          <ProductStatusBadges
            isPublished={row.original.isPublished}
            isFeatured={row.original.isFeatured}
            isDeleted={row.original.isDeleted}
          />
        ),
      },
      {
        id: "actions",
        header: t("products.table.headers.actions"),
        cell: ({ row }) => (
          <div className="flex justify-end">
            <ProductActions product={row.original} />
          </div>
        ),
      },
    ],
    [i18n.resolvedLanguage, t]
  );
}
