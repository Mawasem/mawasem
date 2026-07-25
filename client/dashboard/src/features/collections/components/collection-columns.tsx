import { useMemo } from "react";
import type { ColumnDef } from "@tanstack/react-table";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";

import type { Collection } from "../types";
import { CollectionActions } from "./collection-actions";

export function useCollectionColumns() {
  const { t, i18n } = useTranslation();

  return useMemo<ColumnDef<Collection>[]>(
    () => [
      {
        accessorKey: "nameAr",
        header: t("collections.table.headers.nameAr"),
      },
      {
        accessorKey: "nameEn",
        header: t("collections.table.headers.nameEn"),
      },
      {
        id: "season",
        header: t("collections.table.headers.season"),
        cell: ({ row }) =>
          i18n.resolvedLanguage === "ar"
            ? row.original.seasonNameAr
            : row.original.seasonNameEn,
      },
      {
        accessorKey: "productCount",
        header: t("categories.table.headers.productCount"),
      },
      {
        accessorKey: "isDeleted",
        header: t("collections.table.headers.status"),
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
                ? t("collections.status.deleted")
                : t("collections.status.active")}
            </Badge>
          );
        },
      },
      {
        id: "actions",
        header: t("collections.table.headers.actions"),
        cell: ({ row }) => (
          <div className="flex justify-end">
            <CollectionActions
              collection={row.original}
            />
          </div>
        ),
      },
    ],
    [i18n.resolvedLanguage, t]
  );
}