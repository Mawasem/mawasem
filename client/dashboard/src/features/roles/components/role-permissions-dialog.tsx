import { useMemo, useState } from "react";
import type { FormEvent } from "react";
import { useTranslation } from "react-i18next";

import { EntityDialog } from "@/components/entity-dialog/entity-dialog";
import { EntityDialogFooter } from "@/components/entity-dialog/entity-dialog-footer";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";

import { useRolePermissionOptions } from "../hooks/use-role-permission-options";
import { useUpdateRolePermissions } from "../hooks/use-update-role-permissions";
import type { Role } from "../types/role";

interface RolePermissionsDialogProps {
  role: Role;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function RolePermissionsDialog({
  role,
  open,
  onOpenChange,
}: RolePermissionsDialogProps) {
  const { t } = useTranslation();
  const { data: permissionOptionsData, isLoading: isLoadingOptions } =
    useRolePermissionOptions();
  const { mutateAsync, isLoading: isSaving, error } =
    useUpdateRolePermissions();

  const [selectedPermissionNames, setSelectedPermissionNames] =
    useState<string[]>(role.permissionNames);

  const permissionOptions = useMemo(
    () => permissionOptionsData?.items ?? [],
    [permissionOptionsData]
  );

  const handleTogglePermission = (permissionName: string) => {
    setSelectedPermissionNames((current) => {
      if (current.includes(permissionName)) {
        return current.filter((name) => name !== permissionName);
      }

      return [...current, permissionName];
    });
  };

  const handleSubmit = async (event?: FormEvent<HTMLFormElement>) => {
    event?.preventDefault();

    try {
      await mutateAsync({
        roleName: role.name,
        permissionNames: selectedPermissionNames,
      });

      onOpenChange(false);
    } catch {
      // Keep the dialog open so the error can be displayed.
    }
  };

  return (
    <EntityDialog
      open={open}
      onOpenChange={onOpenChange}
      title={t("roles.dialog.title", { roleName: role.name })}
      description={t("roles.dialog.description")}
    >
      <form
        id="role-permissions-form"
        onSubmit={handleSubmit}
        className="flex max-h-[85vh] flex-col overflow-hidden"
      >
        <div className="flex-shrink-0 space-y-4">
          <div className="rounded-md border p-3 text-sm text-muted-foreground">
            <p>{t("roles.dialog.roleSummary")}</p>
          </div>

          {isLoadingOptions ? (
            <p className="text-sm text-muted-foreground">
              {t("roles.loading")}
            </p>
          ) : null}
        </div>

        <div className="mt-4 flex-1 overflow-y-auto overflow-x-hidden pr-2 scroll-smooth">
          <div className="grid gap-3">
            {permissionOptions.map((permission) => {
              const isSelected = selectedPermissionNames.includes(permission.name);

              return (
                <div
                  key={permission.name}
                  className="flex items-start justify-between gap-3 rounded-md border p-3"
                >
                  <div className="space-y-1">
                    <div className="flex items-center gap-2">
                      <span className="font-medium">
                        {permission.name}
                      </span>
                      {permission.isRequired ? (
                        <Badge variant="secondary">
                          {t("roles.permissions.required")}
                        </Badge>
                      ) : null}
                    </div>
                    <p className="text-sm text-muted-foreground">
                      {permission.description}
                    </p>
                  </div>

                  <Button
                    type="button"
                    variant={isSelected ? "default" : "outline"}
                    size="sm"
                    onClick={() => handleTogglePermission(permission.name)}
                    disabled={isSaving || permission.isRequired}
                  >
                    {isSelected
                      ? t("roles.permissions.selected")
                      : t("roles.permissions.available")}
                  </Button>
                </div>
              );
            })}
          </div>
        </div>

        <div className="mt-4 flex-shrink-0 space-y-4">
          {error instanceof Error ? (
            <p className="text-sm text-destructive">
              {t("roles.errors.generic", { message: error.message })}
            </p>
          ) : null}

          <EntityDialogFooter
            mode="edit"
            formId="role-permissions-form"
            isLoading={isSaving}
            onCancel={() => onOpenChange(false)}
            cancelLabel={t("common.cancel")}
            editLabel={t("roles.actions.savePermissions")}
            editLoadingLabel={t("common.saving")}
          />
        </div>
      </form>
    </EntityDialog>
  );
}
