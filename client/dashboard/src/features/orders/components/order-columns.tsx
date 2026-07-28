import type { ColumnDef } from "@tanstack/react-table";
import { useMemo } from "react";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";
import {
  formatOrderDate,
  formatOrderMoney,
  getDeliveryMethodKey,
  getPaymentStatusKey,
} from "../order-utils";
import type { AdminOrderListItem } from "../types";
import { OrderActions } from "./order-actions";
import { OrderStatusBadge } from "./order-status-badge";

export function useOrderColumns() {
  const { t, i18n } = useTranslation();
  const language = i18n.resolvedLanguage ?? "en";

  return useMemo<ColumnDef<AdminOrderListItem>[]>(() => [
    {
      accessorKey: "orderNumber",
      header: t("orders.table.orderNumber"),
      cell: ({ row }) => <span className="font-mono font-medium">{row.original.orderNumber}</span>,
    },
    {
      id: "customer",
      header: t("orders.table.customer"),
      cell: ({ row }) => (
        <div className="min-w-44">
          <p className="font-medium">{language === "ar" ? row.original.customerNameAr : row.original.customerNameEn}</p>
          <p className="text-xs text-muted-foreground">{row.original.customerPhone}</p>
        </div>
      ),
    },
    {
      accessorKey: "orderStatus",
      header: t("orders.table.status"),
      cell: ({ row }) => <OrderStatusBadge status={row.original.orderStatus} />,
    },
    {
      id: "payment",
      header: t("orders.table.payment"),
      cell: ({ row }) => (
        <Badge variant="outline">{t(`orders.paymentStatus.${getPaymentStatusKey(row.original.paymentStatus)}`)}</Badge>
      ),
    },
    {
      accessorKey: "deliveryMethod",
      header: t("orders.table.deliveryMethod"),
      cell: ({ row }) => t(`orders.deliveryMethod.${getDeliveryMethodKey(row.original.deliveryMethod)}`),
    },
    {
      id: "items",
      header: t("orders.table.items"),
      cell: ({ row }) => (
        <div className="whitespace-nowrap text-sm">
          <p>{t("orders.table.distinctItems", { count: row.original.distinctItemCount })}</p>
          <p className="text-xs text-muted-foreground">{t("orders.table.totalQuantity", { count: row.original.totalQuantity })}</p>
        </div>
      ),
    },
    {
      accessorKey: "totalAmount",
      header: t("orders.table.total"),
      cell: ({ row }) => <span className="whitespace-nowrap font-semibold">{formatOrderMoney(row.original.totalAmount, language)}</span>,
    },
    {
      accessorKey: "orderDate",
      header: t("orders.table.date"),
      cell: ({ row }) => <span className="whitespace-nowrap text-sm">{formatOrderDate(row.original.orderDate, language)}</span>,
    },
    {
      id: "actions",
      header: t("orders.table.actions"),
      cell: ({ row }) => <div className="flex justify-end"><OrderActions order={row.original} /></div>,
    },
  ], [language, t]);
}
