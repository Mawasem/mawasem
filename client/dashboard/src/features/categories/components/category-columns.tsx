import { useMemo } from "react";
import type { ColumnDef } from "@tanstack/react-table";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";

import type { Category } from "../types/category";
import { CategoryActions } from "./category-actions";

export function useCategoryColumns() {
  const { t } = useTranslation();

  return useMemo<ColumnDef<Category>[]>(
    () => [
      {
        accessorKey: "nameAr",
        header: t("categories.table.headers.nameAr"),
      },
      {
        accessorKey: "nameEn",
        header: t("categories.table.headers.nameEn"),
      },
      {
        accessorKey: "productCount",
        header: t("categories.table.headers.productCount"),
      },
      {
        accessorKey: "isDeleted",
        header: t("categories.table.headers.status"),
        cell: ({ row }) => {
          const isDeleted = row.original.isDeleted;

          return (
            <Badge
              variant={
                isDeleted
                  ? "secondary"
                  : "default"
              }
            >
              {isDeleted
                ? t("categories.status.deleted")
                : t("categories.status.active")}
            </Badge>
          );
        },
      },
      {
        id: "actions",
        header: t("categories.table.headers.actions"),
        cell: ({ row }) => (
          <div className="flex justify-end">
            <CategoryActions
              category={row.original}
            />
          </div>
        ),
      },
    ],
    [t]
  );
}
