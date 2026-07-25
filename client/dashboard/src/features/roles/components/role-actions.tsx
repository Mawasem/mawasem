import { MoreHorizontal } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import type { Role } from "../types/role";
import { RolePermissionsDialog } from "./role-permissions-dialog";

interface RoleActionsProps {
  role: Role;
}

export function RoleActions({ role }: RoleActionsProps) {
  const { t } = useTranslation();
  const [isPermissionsDialogOpen, setIsPermissionsDialogOpen] =
    useState(false);

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            size="icon-sm"
            aria-label={t("roles.actions.openActions")}
          >
            <MoreHorizontal className="size-4" />
          </Button>
        </DropdownMenuTrigger>

        <DropdownMenuContent align="end">
          <DropdownMenuItem
            onClick={() => setIsPermissionsDialogOpen(true)}
            disabled={role.isProtected}
          >
            {t("roles.actions.managePermissions")}
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>

      <RolePermissionsDialog
        key={`${role.name}-${isPermissionsDialogOpen ? "open" : "closed"}`}
        role={role}
        open={isPermissionsDialogOpen}
        onOpenChange={setIsPermissionsDialogOpen}
      />
    </>
  );
}
