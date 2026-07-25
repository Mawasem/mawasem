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

import { useDeleteCollection } from "../hooks/use-delete-collection";
import { useRestoreCollection } from "../hooks/use-restore-collection";
import { CollectionDialog } from "./collection-dialog";
import type { CollectionActionsProps } from "./types";

export function CollectionActions({
  collection,
}: CollectionActionsProps) {
  const { t, i18n } = useTranslation();

  const [isDeleteDialogOpen, setIsDeleteDialogOpen] =
    useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    useState(false);

  const {
    deleteCollectionAsync,
    isLoading: isDeleting,
    error: deleteError,
  } = useDeleteCollection();

  const {
    restoreCollectionAsync,
    isLoading: isRestoring,
  } = useRestoreCollection();

  const deleteErrorMessage =
    deleteError instanceof Error
      ? deleteError.message
      : null;

  const handleDelete = async () => {
    try {
      await deleteCollectionAsync(collection.id);
      setIsDeleteDialogOpen(false);
    } catch {
      // Error is shown in the dialog.
    }
  };

  const handleRestore = async () => {
    try {
      await restoreCollectionAsync(collection.id);
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
          {collection.isDeleted ? (
            <DropdownMenuItem
              onClick={() => {
                void handleRestore();
              }}
              disabled={isRestoring}
            >
              {isRestoring
                ? t("common.restoring")
                : t("collections.actions.restore")}
            </DropdownMenuItem>
          ) : (
            <>
              <DropdownMenuItem
                onClick={() => setIsEditDialogOpen(true)}
              >
                {t("collections.actions.edit")}
              </DropdownMenuItem>

              <DropdownMenuItem
                variant="destructive"
                onClick={() => setIsDeleteDialogOpen(true)}
              >
                {t("collections.actions.delete")}
              </DropdownMenuItem>
            </>
          )}
        </DropdownMenuContent>
      </DropdownMenu>

      <CollectionDialog
        mode="edit"
        collection={collection}
        open={isEditDialogOpen}
        onOpenChange={setIsEditDialogOpen}
      />

      <DeleteEntityDialog
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
        title={t("collections.delete.title")}
        description={t("collections.delete.description")}
        entityName={
          i18n.resolvedLanguage === "ar"
            ? collection.nameAr
            : collection.nameEn
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
