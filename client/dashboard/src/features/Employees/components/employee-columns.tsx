import { useMemo } from "react"
import type { ColumnDef } from "@tanstack/react-table"
import { useTranslation } from "react-i18next"

import { Badge } from "@/components/ui/badge"

import type { Employee } from "../types"
import { EmployeeActions } from "./employee-actions"

export function useEmployeeColumns() {
  const { t } = useTranslation()

  return useMemo<ColumnDef<Employee>[]>(
    () => [
      {
        accessorKey: "fullNameAr",
        header: t("employees.table.headers.fullNameAr"),
      },
      {
        accessorKey: "fullNameEn",
        header: t("employees.table.headers.fullNameEn"),
      },
      {
        accessorKey: "email",
        header: t("employees.table.headers.email"),
      },
      {
        id: "roles",
        header: t("employees.table.headers.roles"),
        cell: ({ row }) => {
          const roles = row.original.roles

          if (roles.length === 0) {
            return (
              <span className="text-muted-foreground">
                {t("common.notAvailable")}
              </span>
            )
          }

          return (
            <div className="flex max-w-xs flex-wrap gap-1">
              {roles.map((role) => (
                <Badge key={role} variant="outline">
                  {role}
                </Badge>
              ))}
            </div>
          )
        },
      },
      {
        id: "effectivePermissionCount",
        header: t("employees.table.headers.effectivePermissionCount"),
        cell: ({ row }) => {
          const permissionCount = row.original.effectivePermissions.length

          return (
            <Badge variant="secondary">
              {t("employees.table.effectivePermissionsCount", {
                count: permissionCount,
              })}
            </Badge>
          )
        },
      },
      {
        accessorKey: "mustChangePassword",
        header: t("employees.table.headers.passwordStatus"),
        cell: ({ row }) => {
          const mustChangePassword = row.original.mustChangePassword

          return (
            <Badge variant={mustChangePassword ? "secondary" : "outline"}>
              {mustChangePassword
                ? t("employees.passwordStatus.changeRequired")
                : t("employees.passwordStatus.ready")}
            </Badge>
          )
        },
      },
      {
        accessorKey: "isBlocked",
        header: t("employees.table.headers.status"),
        cell: ({ row }) => {
          const isBlocked = row.original.isBlocked

          return (
            <Badge variant={isBlocked ? "destructive" : "default"}>
              {isBlocked
                ? t("employees.status.blocked")
                : t("employees.status.active")}
            </Badge>
          )
        },
      },
      {
        id: "actions",
        header: t("employees.table.headers.actions"),
        cell: ({ row }) => (
          <div className="flex justify-end">
            <EmployeeActions employee={row.original} />
          </div>
        ),
      },
    ],
    [t]
  )
}
