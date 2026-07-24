import { MoreHorizontal } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";

import { DeleteEntityDialog } from "@/components/entity-dialog/delete-entity-dialog";
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
  } = useRestoreCategory();

  const deleteErrorMessage =
    deleteError instanceof Error
      ? deleteError.message
      : null;

  const handleDelete = async () => {
    try {
      await deleteCategoryMutationAsync(category.id);
      setIsDeleteDialogOpen(false);
    } catch {
      // Error is shown in the dialog.
    }
  };

  const handleRestore = async () => {
    try {
      await restoreCategoryMutationAsync(category.id);
    } catch {
      // Mutation error is surfaced by React Query.
    }
  };

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
              onClick={() => {
                void handleRestore();
              }}
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

      <DeleteEntityDialog
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
        title={t("categories.deleteDialog.title")}
        description={t("categories.deleteDialog.description")}
        entityName={
          i18n.resolvedLanguage === "ar"
            ? category.nameAr
            : category.nameEn
        }
        isDeleting={isDeleting}
        errorMessage={deleteErrorMessage}
        confirmLabel={t("common.delete")}
        deletingLabel={t("common.deleting")}
        cancelLabel={t("common.cancel")}
        onConfirm={handleDelete}
      />
    </>
  );
}