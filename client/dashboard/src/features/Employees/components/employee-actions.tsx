import { MoreHorizontal } from "lucide-react"
import { useState } from "react"
import { useTranslation } from "react-i18next"

import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"

import type { EmployeeActionsProps } from "../types"
import { BlockEmployeeDialog } from "./block-employee-dialog"
import { EmployeeDetailsDialog } from "./employee-details-dialog"
import { EmployeeDialog } from "./employee-dialog"
import { ManageEmployeePermissionsDialog } from "./manage-employee-permissions-dialog"
import { ManageEmployeeRolesDialog } from "./manage-employee-roles-dialog"
import { ResetEmployeePasswordDialog } from "./reset-employee-password-dialog"
import { UnblockEmployeeDialog } from "./unblock-employee-dialog"

export function EmployeeActions({ employee }: EmployeeActionsProps) {
  const { t } = useTranslation()

  const [isDetailsDialogOpen, setIsDetailsDialogOpen] = useState(false)

  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false)

  const [isBlockDialogOpen, setIsBlockDialogOpen] = useState(false)

  const [isUnblockDialogOpen, setIsUnblockDialogOpen] = useState(false)

  const [isResetPasswordDialogOpen, setIsResetPasswordDialogOpen] =
    useState(false)

  const [isManageRolesDialogOpen, setIsManageRolesDialogOpen] = useState(false)

  const [isManagePermissionsDialogOpen, setIsManagePermissionsDialogOpen] =
    useState(false)

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            size="icon-sm"
            aria-label={t("employees.actions.openActions")}
          >
            <MoreHorizontal className="size-4" />
          </Button>
        </DropdownMenuTrigger>

        <DropdownMenuContent align="end">
          <DropdownMenuItem onClick={() => setIsDetailsDialogOpen(true)}>
            {t("employees.actions.viewDetails")}
          </DropdownMenuItem>

          <DropdownMenuItem onClick={() => setIsEditDialogOpen(true)}>
            {t("employees.actions.edit")}
          </DropdownMenuItem>

          <DropdownMenuSeparator />

          <DropdownMenuItem onClick={() => setIsManageRolesDialogOpen(true)}>
            {t("employees.actions.manageRoles")}
          </DropdownMenuItem>

          <DropdownMenuItem
            onClick={() => setIsManagePermissionsDialogOpen(true)}
          >
            {t("employees.actions.managePermissions")}
          </DropdownMenuItem>

          <DropdownMenuItem onClick={() => setIsResetPasswordDialogOpen(true)}>
            {t("employees.actions.resetPassword")}
          </DropdownMenuItem>

          <DropdownMenuSeparator />

          {employee.isBlocked ? (
            <DropdownMenuItem onClick={() => setIsUnblockDialogOpen(true)}>
              {t("employees.actions.unblock")}
            </DropdownMenuItem>
          ) : (
            <DropdownMenuItem
              variant="destructive"
              onClick={() => setIsBlockDialogOpen(true)}
            >
              {t("employees.actions.block")}
            </DropdownMenuItem>
          )}
        </DropdownMenuContent>
      </DropdownMenu>

      <EmployeeDetailsDialog
        employee={employee}
        open={isDetailsDialogOpen}
        onOpenChange={setIsDetailsDialogOpen}
      />

      <EmployeeDialog
        mode="edit"
        employee={employee}
        open={isEditDialogOpen}
        onOpenChange={setIsEditDialogOpen}
      />

      <ManageEmployeeRolesDialog
        employee={employee}
        open={isManageRolesDialogOpen}
        onOpenChange={setIsManageRolesDialogOpen}
      />

      <ManageEmployeePermissionsDialog
        employee={employee}
        open={isManagePermissionsDialogOpen}
        onOpenChange={setIsManagePermissionsDialogOpen}
      />

      <ResetEmployeePasswordDialog
        employee={employee}
        open={isResetPasswordDialogOpen}
        onOpenChange={setIsResetPasswordDialogOpen}
      />

      <BlockEmployeeDialog
        employee={employee}
        open={isBlockDialogOpen}
        onOpenChange={setIsBlockDialogOpen}
      />

      <UnblockEmployeeDialog
        employee={employee}
        open={isUnblockDialogOpen}
        onOpenChange={setIsUnblockDialogOpen}
      />
    </>
  )
}
