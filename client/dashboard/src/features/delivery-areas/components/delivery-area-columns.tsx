import type { ColumnDef } from "@tanstack/react-table";
import { useMemo } from "react";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";

import type { DeliveryArea } from "../types";
import { DeliveryAreaActions } from "./delivery-area-actions";
import { DeliveryAreaStatusBadge } from "./delivery-area-status-badge";

export function useDeliveryAreaColumns() {
  const { t, i18n } = useTranslation();

  return useMemo<ColumnDef<DeliveryArea>[]>(() => {
    const locale =
      i18n.resolvedLanguage === "ar" ? "ar-EG" : "en-GB";

    const formatAmount = (value: number) =>
      new Intl.NumberFormat(locale, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }).format(value);

    return [
      {
        accessorKey: "nameAr",
        header: t("deliveryAreas.table.headers.nameAr"),
      },
      {
        accessorKey: "nameEn",
        header: t("deliveryAreas.table.headers.nameEn"),
      },
      {
        accessorKey: "status",
        header: t("deliveryAreas.table.headers.status"),
        cell: ({ row }) => (
          <DeliveryAreaStatusBadge status={row.original.status} />
        ),
      },
      {
        id: "fees",
        header: t("deliveryAreas.table.headers.fees"),
        cell: ({ row }) => (
          <div className="space-y-1 whitespace-nowrap">
            <p className="text-sm font-medium">
              {formatAmount(row.original.effectiveDeliveryFee)}
            </p>
            {!row.original.isFreeDelivery ? (
              <p className="text-xs text-muted-foreground">
                {t("deliveryAreas.table.configuredFee", {
                  amount: formatAmount(row.original.deliveryFee),
                })}
              </p>
            ) : null}
          </div>
        ),
      },
      {
        accessorKey: "isFreeDelivery",
        header: t("deliveryAreas.table.headers.freeDelivery"),
        cell: ({ row }) => (
          <Badge variant={row.original.isFreeDelivery ? "default" : "outline"}>
            {row.original.isFreeDelivery
              ? t("common.yes")
              : t("common.no")}
          </Badge>
        ),
      },
      {
        accessorKey: "isActive",
        header: t("deliveryAreas.table.headers.activity"),
        cell: ({ row }) => {
          if (row.original.isDeleted) {
            return (
              <Badge variant="destructive">
                {t("deliveryAreas.activity.deleted")}
              </Badge>
            );
          }

          return (
            <Badge variant={row.original.isActive ? "default" : "secondary"}>
              {row.original.isActive
                ? t("deliveryAreas.activity.active")
                : t("deliveryAreas.activity.inactive")}
            </Badge>
          );
        },
      },
      {
        accessorKey: "activeAddressCount",
        header: t("deliveryAreas.table.headers.activeAddresses"),
        cell: ({ row }) => (
          <Badge
            variant={
              row.original.activeAddressCount > 0 ? "secondary" : "outline"
            }
          >
            {row.original.activeAddressCount}
          </Badge>
        ),
      },
      {
        id: "actions",
        header: t("deliveryAreas.table.headers.actions"),
        cell: ({ row }) => (
          <div className="flex justify-end">
            <DeliveryAreaActions deliveryArea={row.original} />
          </div>
        ),
      },
    ];
  }, [i18n.resolvedLanguage, t]);
}
