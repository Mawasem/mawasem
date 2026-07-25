import { MoreHorizontal } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";

import { EntityDeleteDialog } from "@/components/entity-dialog/EntityDeleteDialog";
import { EntityRestoreDialog } from "@/components/entity-dialog/EntityRestoreDialog";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import { useDeleteCategory } from "../hooks/use-delete-category";
import { useRestoreCategory } from "../hooks/use-restore-category";
import { CategoryDialog } from "./category-dialog";
import type { CategoryActionsProps } from "./types";

export function CategoryActions({
  category,
}: CategoryActionsProps) {
  const { t, i18n } = useTranslation();

  const [isDeleteDialogOpen, setIsDeleteDialogOpen] =
    useState(false);
  const [isRestoreDialogOpen, setIsRestoreDialogOpen] =
    useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    useState(false);

  const {
    deleteCategoryMutationAsync,
    isLoading: isDeleting,
    error: deleteError,
  } = useDeleteCategory();

  const {
    restoreCategoryMutationAsync,
    isLoading: isRestoring,
    error: restoreError,
  } = useRestoreCategory();

  const deleteMutation = {
    mutateAsync: deleteCategoryMutationAsync,
    isLoading: isDeleting,
    error: deleteError,
  };

  const restoreMutation = {
    mutateAsync: restoreCategoryMutationAsync,
    isLoading: isRestoring,
    error: restoreError,
  };

  const entityName =
    i18n.resolvedLanguage === "ar"
      ? category.nameAr
      : category.nameEn;

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            size="icon-sm"
            aria-label={t("categories.actions.openActions")}
          >
            <MoreHorizontal className="size-4" />
          </Button>
        </DropdownMenuTrigger>

        <DropdownMenuContent align="end">
          {category.isDeleted ? (
            <DropdownMenuItem
              onClick={() => setIsRestoreDialogOpen(true)}
              disabled={isRestoring}
            >
              {isRestoring
                ? t("common.restoring")
                : t("categories.actions.restore")}
            </DropdownMenuItem>
          ) : (
            <>
              <DropdownMenuItem
                onClick={() => setIsEditDialogOpen(true)}
              >
                {t("categories.actions.edit")}
              </DropdownMenuItem>

              <DropdownMenuItem
                variant="destructive"
                onClick={() => setIsDeleteDialogOpen(true)}
              >
                {t("categories.actions.delete")}
              </DropdownMenuItem>
            </>
          )}
        </DropdownMenuContent>
      </DropdownMenu>

      <CategoryDialog
        mode="edit"
        category={category}
        open={isEditDialogOpen}
        onOpenChange={setIsEditDialogOpen}
      />

      <EntityDeleteDialog
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
        title={t("categories.deleteDialog.title")}
        description={t("categories.deleteDialog.description")}
        entityName={entityName}
        confirmLabel={t("common.delete")}
        loadingLabel={t("common.deleting")}
        cancelLabel={t("common.cancel")}
        mutation={deleteMutation}
        entityId={category.id}
      />

      <EntityRestoreDialog
        open={isRestoreDialogOpen}
        onOpenChange={setIsRestoreDialogOpen}
        title={t("categories.restoreDialog.title")}
        description={t("categories.restoreDialog.description")}
        entityName={entityName}
        confirmLabel={t("categories.actions.restore")}
        loadingLabel={t("common.restoring")}
        cancelLabel={t("common.cancel")}
        mutation={restoreMutation}
        entityId={category.id}
      />
    </>
  );
}