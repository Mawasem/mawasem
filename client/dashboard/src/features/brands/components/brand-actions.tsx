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

import { useDeleteBrand } from "../hooks/use-delete-brand";
import { useRestoreBrand } from "../hooks/use-restore-brand";
import { BrandDialog } from "./brand-dialog";
import type { BrandActionsProps } from "./types";
import type { Brand } from "../types/brand";

interface BrandWithLegacyDeleted extends Brand {
  IsDeleted?: boolean;
}

export function BrandActions({
  brand,
}: BrandActionsProps) {
  const { i18n } = useTranslation();

  const [isDeleteDialogOpen, setIsDeleteDialogOpen] =
    useState(false);
  const [isRestoreDialogOpen, setIsRestoreDialogOpen] =
    useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    useState(false);

  const deleteBrandMutation = useDeleteBrand();
  const {
    restoreBrandMutationAsync,
    isLoading: isRestoring,
    error: restoreError,
  } = useRestoreBrand();

  const restoreBrandMutation = {
    mutateAsync: restoreBrandMutationAsync,
    isLoading: isRestoring,
    error: restoreError,
  };

  const entityName =
    i18n.resolvedLanguage === "ar"
      ? brand.nameAr
      : brand.nameEn;
  const isDeleted = Boolean(
    (brand as BrandWithLegacyDeleted).isDeleted ??
      (brand as BrandWithLegacyDeleted).IsDeleted
  );

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
          {isDeleted ? (
            <DropdownMenuItem
              onClick={() => setIsRestoreDialogOpen(true)}
              disabled={isRestoring}
            >
              {isRestoring
                ? "Restoring..."
                : "Restore Brand"}
            </DropdownMenuItem>
          ) : (
            <>
              <DropdownMenuItem
                onClick={() =>
                  setIsEditDialogOpen(true)
                }
              >
                Edit Brand
              </DropdownMenuItem>

              <DropdownMenuItem
                variant="destructive"
                onClick={() =>
                  setIsDeleteDialogOpen(true)
                }
              >
                Delete Brand
              </DropdownMenuItem>
            </>
          )}
        </DropdownMenuContent>
      </DropdownMenu>

      <BrandDialog
        mode="edit"
        brand={brand}
        open={isEditDialogOpen}
        onOpenChange={setIsEditDialogOpen}
      />

      <EntityDeleteDialog
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
        title="Delete Brand"
        description="Are you sure you want to delete this brand? This action cannot be undone."
        entityName={entityName}
        confirmLabel="Delete"
        loadingLabel="Deleting..."
        cancelLabel="Cancel"
        mutation={deleteBrandMutation}
        entityId={brand.id}
      />

      <EntityRestoreDialog
        open={isRestoreDialogOpen}
        onOpenChange={setIsRestoreDialogOpen}
        title="Restore Brand"
        description="Are you sure you want to restore this brand?"
        entityName={entityName}
        confirmLabel="Restore"
        loadingLabel="Restoring..."
        cancelLabel="Cancel"
        mutation={restoreBrandMutation}
        entityId={brand.id}
      />
    </>
  );
}
