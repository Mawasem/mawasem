import { useMemo } from "react";
import type { ColumnDef } from "@tanstack/react-table";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";

import type { Season } from "../types";
import { SeasonActions } from "./season-actions";

export function useSeasonColumns() {
  const { t, i18n } = useTranslation();

  return useMemo<ColumnDef<Season>[]>(
    () => [
      {
        accessorKey: "nameAr",
        header: t("seasons.table.headers.nameAr"),
      },

      {
        accessorKey: "nameEn",
        header: t("seasons.table.headers.nameEn"),
      },

      {
        accessorKey: "descriptionAr",
        header: t("seasons.table.headers.description"),

        cell: ({ row }) => {
          const description =
            i18n.resolvedLanguage === "ar"
              ? row.original.descriptionAr
              : row.original.descriptionEn;

          return (
            <span className="line-clamp-1 max-w-[250px]">
              {description}
            </span>
          );
        },
      },

      {
        accessorKey: "isActive",
        header: t("seasons.table.headers.status"),

        cell: ({ row }) => {
          const isActive = row.original.isActive;
          const isDeleted = Boolean(
            (row.original as { isDeleted?: boolean; IsDeleted?: boolean }).isDeleted ??
              (row.original as { isDeleted?: boolean; IsDeleted?: boolean }).IsDeleted
          );

          return (
            <Badge
              variant={isDeleted ? "secondary" : isActive ? "default" : "secondary"}
            >
              {isDeleted
                ? t("seasons.status.deleted")
                : isActive
                  ? t("seasons.status.active")
                  : t("seasons.status.inactive")}
            </Badge>
          );
        },
      },

      {
        id: "actions",
        header: t("seasons.table.headers.actions"),

        cell: ({ row }) => (
          <div className="flex justify-end">
            <SeasonActions
              season={row.original}
            />
          </div>
        ),
      },
    ],
    [i18n.resolvedLanguage, t]
  );
}
