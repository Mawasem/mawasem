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

import { useDeleteSeason } from "../hooks/use-delete-season";
import { useRestoreSeason } from "../hooks/use-restore-season";
import type { Season, SeasonActionsProps } from "../types";
import { SeasonDialog } from "./season-dialog";

interface SeasonWithLegacyDeleted extends Season {
  IsDeleted?: boolean;
}

export function SeasonActions({
  season,
}: SeasonActionsProps) {
  const { t, i18n } = useTranslation();

  const [isDeleteDialogOpen, setIsDeleteDialogOpen] =
    useState(false);
  const [isRestoreDialogOpen, setIsRestoreDialogOpen] =
    useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    useState(false);

  const deleteSeasonMutation = useDeleteSeason();
  const {
    restoreSeasonMutationAsync,
    isLoading: isRestoring,
    error: restoreError,
  } = useRestoreSeason();

  const restoreSeasonMutation = {
    mutateAsync: restoreSeasonMutationAsync,
    isLoading: isRestoring,
    error: restoreError,
  };

  const entityName =
    i18n.resolvedLanguage === "ar"
      ? season.nameAr
      : season.nameEn;
  const isDeleted = Boolean(
    (season as SeasonWithLegacyDeleted).isDeleted ??
      (season as SeasonWithLegacyDeleted).IsDeleted
  );

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            size="icon-sm"
            aria-label={t("seasons.actions.openActions")}
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
                ? t("common.restoring")
                : t("seasons.actions.restore")}
            </DropdownMenuItem>
          ) : (
            <>
              <DropdownMenuItem
                onClick={() =>
                  setIsEditDialogOpen(true)
                }
              >
                {t("seasons.actions.edit")}
              </DropdownMenuItem>

              <DropdownMenuItem
                variant="destructive"
                onClick={() =>
                  setIsDeleteDialogOpen(true)
                }
              >
                {t("seasons.actions.delete")}
              </DropdownMenuItem>
            </>
          )}
        </DropdownMenuContent>
      </DropdownMenu>

      <SeasonDialog
        mode="edit"
        season={season}
        open={isEditDialogOpen}
        onOpenChange={setIsEditDialogOpen}
      />

      <EntityDeleteDialog
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
        title={t("seasons.deleteDialog.title")}
        description={t("seasons.deleteDialog.description")}
        entityName={entityName}
        confirmLabel={t("common.delete")}
        loadingLabel={t("common.deleting")}
        cancelLabel={t("common.cancel")}
        mutation={deleteSeasonMutation}
        entityId={season.id}
      />

      <EntityRestoreDialog
        open={isRestoreDialogOpen}
        onOpenChange={setIsRestoreDialogOpen}
        title={t("seasons.restoreDialog.title")}
        description={t("seasons.restoreDialog.description")}
        entityName={entityName}
        confirmLabel={t("seasons.actions.restore")}
        loadingLabel={t("common.restoring")}
        cancelLabel={t("common.cancel")}
        mutation={restoreSeasonMutation}
        entityId={season.id}
      />
    </>
  );
}
