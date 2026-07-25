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
  const [isRestoreDialogOpen, setIsRestoreDialogOpen] =
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
    error: restoreError,
  } = useRestoreCollection();

  const deleteMutation = {
    mutateAsync: deleteCollectionAsync,
    isLoading: isDeleting,
    error: deleteError,
  };

  const restoreMutation = {
    mutateAsync: restoreCollectionAsync,
    isLoading: isRestoring,
    error: restoreError,
  };

  const entityName =
    i18n.resolvedLanguage === "ar"
      ? collection.nameAr
      : collection.nameEn;

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            size="icon-sm"
            aria-label={t("collections.actions.openActions")}
          >
            <MoreHorizontal className="size-4" />
          </Button>
        </DropdownMenuTrigger>

        <DropdownMenuContent align="end">
          {collection.isDeleted ? (
            <DropdownMenuItem
              onClick={() => setIsRestoreDialogOpen(true)}
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

      <EntityDeleteDialog
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
        title={t("collections.delete.title")}
        description={t("collections.delete.description")}
        entityName={entityName}
        confirmLabel={t("common.delete")}
        loadingLabel={t("common.deleting")}
        cancelLabel={t("common.cancel")}
        mutation={deleteMutation}
        entityId={collection.id}
      />

      <EntityRestoreDialog
        open={isRestoreDialogOpen}
        onOpenChange={setIsRestoreDialogOpen}
        title={t("collections.restore.title")}
        description={t("collections.restore.description")}
        entityName={entityName}
        confirmLabel={t("collections.restore.confirm")}
        loadingLabel={t("common.restoring")}
        cancelLabel={t("common.cancel")}
        mutation={restoreMutation}
        entityId={collection.id}
      />
    </>
  );
}
