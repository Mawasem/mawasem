import { MoreHorizontal } from "lucide-react";
import { useState } from "react";

import {
  AlertDialog,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import { useDeleteSeason } from "../hooks/use-delete-season";
import type { SeasonActionsProps } from "../types";
import { SeasonDialog } from "./season-dialog";

export function SeasonActions({
  season,
}: SeasonActionsProps) {
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] =
    useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    useState(false);

  const deleteSeasonMutation = useDeleteSeason();

  const handleDelete = async () => {
    try {
      await deleteSeasonMutation.mutateAsync(season.id);
      setIsDeleteDialogOpen(false);
    } catch {
      // Error is shown in the dialog body.
    }
  };

  const errorMessage =
    deleteSeasonMutation.error instanceof Error
      ? deleteSeasonMutation.error.message
      : "Failed to delete season. Please try again.";

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
          <DropdownMenuItem
            onClick={() =>
              setIsEditDialogOpen(true)
            }
          >
            Edit Season
          </DropdownMenuItem>

          <DropdownMenuItem
            variant="destructive"
            onClick={() =>
              setIsDeleteDialogOpen(true)
            }
          >
            Delete Season
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>

      <SeasonDialog
        mode="edit"
        season={season}
        open={isEditDialogOpen}
        onOpenChange={setIsEditDialogOpen}
      />

      <AlertDialog
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>
              Delete season
            </AlertDialogTitle>

            <AlertDialogDescription>
              Are you sure you want to delete this season? This action cannot be undone.
            </AlertDialogDescription>

            {deleteSeasonMutation.isError ? (
              <p className="text-sm text-destructive">
                {errorMessage}
              </p>
            ) : null}
          </AlertDialogHeader>

          <AlertDialogFooter>
            <AlertDialogCancel asChild>
              <Button
                variant="outline"
                disabled={deleteSeasonMutation.isPending}
              >
                Cancel
              </Button>
            </AlertDialogCancel>

            <Button
              variant="destructive"
              onClick={handleDelete}
              disabled={deleteSeasonMutation.isPending}
            >
              {deleteSeasonMutation.isPending
                ? "Deleting..."
                : "Delete"}
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
