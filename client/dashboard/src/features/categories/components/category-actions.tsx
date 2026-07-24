import { MoreHorizontal } from "lucide-react";
import { useState } from "react";

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
            aria-label="Open actions"
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
              {isRestoring ? "Restoring..." : "Restore Category"}
            </DropdownMenuItem>
          ) : (
            <>
              <DropdownMenuItem
                onClick={() => setIsEditDialogOpen(true)}
              >
                Edit Category
              </DropdownMenuItem>

              <DropdownMenuItem
                variant="destructive"
                onClick={() => setIsDeleteDialogOpen(true)}
              >
                Delete Category
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
        title="Delete Category"
        description="Are you sure you want to delete this category?\n\nThis action can be reversed later by restoring the category."
        entityName={category.nameEn}
        isDeleting={isDeleting}
        errorMessage={deleteErrorMessage}
        confirmLabel="Delete"
        cancelLabel="Cancel"
        onConfirm={handleDelete}
      />
    </>
  );
}