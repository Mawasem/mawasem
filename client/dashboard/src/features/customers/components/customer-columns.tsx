import { useMemo } from "react";
import type { ColumnDef } from "@tanstack/react-table";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";

import type { Customer } from "../types";
import { CustomerActions } from "./customer-actions";

export function useCustomerColumns() {
  const { t, i18n } = useTranslation();

  return useMemo<ColumnDef<Customer>[]>(
    () => [
      {
        accessorKey: "fullNameAr",
        header: t("customers.table.headers.fullNameAr"),
      },
      {
        accessorKey: "fullNameEn",
        header: t("customers.table.headers.fullNameEn"),
      },
      {
        accessorKey: "phoneNumber",
        header: t("customers.table.headers.phoneNumber"),
      },
      {
        accessorKey: "totalOrders",
        header: t("customers.table.headers.totalOrders"),
      },
      {
        accessorKey: "totalSpent",
        header: t("customers.table.headers.totalSpent"),
        cell: ({ row }) => {
          const amount = row.original.totalSpent;

          const locale =
            i18n.resolvedLanguage === "ar"
              ? "ar-EG"
              : "en-US";

          return (
            <span>
              {new Intl.NumberFormat(locale, {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2,
              }).format(amount)}
            </span>
          );
        },
      },
      {
        accessorKey: "isBlocked",
        header: t("customers.table.headers.status"),
        cell: ({ row }) => {
          const isBlocked = row.original.isBlocked;

          return (
            <Badge
              variant={
                isBlocked
                  ? "secondary"
                  : "default"
              }
            >
              {isBlocked
                ? t("customers.status.blocked")
                : t("customers.status.active")}
            </Badge>
          );
        },
      },
      {
        id: "actions",
        header: t("customers.table.headers.actions"),
        cell: ({ row }) => (
          <div className="flex justify-end">
            <CustomerActions
              customer={row.original}
            />
          </div>
        ),
      },
    ],
    [i18n.resolvedLanguage, t]
  );
}
