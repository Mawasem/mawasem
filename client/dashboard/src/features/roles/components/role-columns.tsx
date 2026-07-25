import { useMemo } from "react";
import type { ColumnDef } from "@tanstack/react-table";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";

import type { Role } from "../types/role";
import { RoleActions } from "./role-actions";

export function useRoleColumns() {
  const { t } = useTranslation();

  return useMemo<ColumnDef<Role>[]>(
    () => [
      {
        accessorKey: "name",
        header: t("roles.table.headers.name"),
      },
      {
        accessorKey: "assignedUserCount",
        header: t("roles.table.headers.assignedUsers"),
      },
      {
        accessorKey: "permissionNames",
        header: t("roles.table.headers.permissions"),
        cell: ({ row }) => {
          const permissionCount = row.original.permissionNames.length;

          const summaryKey =
            permissionCount === 0
              ? "roles.table.permissionSummary.empty"
              : permissionCount === 1
                ? "roles.table.permissionSummary.one"
                : "roles.table.permissionSummary.other";

          return (
            <span className="whitespace-nowrap text-sm text-muted-foreground">
              {t(summaryKey, { count: permissionCount })}
            </span>
          );
        },
      },
      {
        accessorKey: "isProtected",
        header: t("roles.table.headers.status"),
        cell: ({ row }) => {
          const role = row.original;

          return (
            <Badge
              variant={role.isProtected ? "secondary" : "default"}
            >
              {role.isProtected
                ? t("roles.status.protected")
                : t("roles.status.manageable")}
            </Badge>
          );
        },
      },
      {
        id: "actions",
        header: t("roles.table.headers.actions"),
        cell: ({ row }) => (
          <div className="flex justify-end">
            <RoleActions role={row.original} />
          </div>
        ),
      },
    ],
    [t]
  );
}
