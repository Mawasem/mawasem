import { EntityDialog } from "@/components/entity-dialog/entity-dialog";
import { EntityDialogFooter } from "@/components/entity-dialog/entity-dialog-footer";

import { useCreateSeason } from "../hooks/use-create-season";
import { useUpdateSeason } from "../hooks/use-update-season";
import type { SeasonFormValues } from "../schema/season-schema";
import type { SeasonDialogProps } from "../types";
import { SeasonForm } from "./season-form";

export function SeasonDialog({
  open,
  onOpenChange,
  mode,
  season,
}: SeasonDialogProps) {
  const createSeasonMutation = useCreateSeason();
  const updateSeasonMutation = useUpdateSeason();

  const isEditMode = mode === "edit";

  const title = isEditMode
    ? "Edit Season"
    : "Add Season";

  const description = isEditMode
    ? "Update season details and save your changes."
    : "Create a new season by filling the details below.";

  const formId = `season-form-${mode}`;

  const isSubmitting =
    createSeasonMutation.isPending ||
    updateSeasonMutation.isPending;

  const mutationError =
    createSeasonMutation.error ??
    updateSeasonMutation.error;

  const errorMessage =
    mutationError instanceof Error
      ? mutationError.message
      : null;

  const handleSubmit = async (
    values: SeasonFormValues
  ) => {
    try {
      if (isEditMode && season) {
        await updateSeasonMutation.mutateAsync({
          id: season.id,
          data: values,
        });
      } else {
        await createSeasonMutation.mutateAsync(values);
      }

      onOpenChange(false);
    } catch {
      // Keep dialog open and show mutation error.
    }
  };

  return (
    <EntityDialog
      open={open}
      onOpenChange={onOpenChange}
      title={title}
      description={description}
    >
      <div className="space-y-5">
        <SeasonForm
          mode={mode}
          season={season}
          formId={formId}
          errorMessage={errorMessage}
          onSubmit={handleSubmit}
        />

        <EntityDialogFooter
          mode={mode}
          formId={formId}
          isLoading={isSubmitting}
          onCancel={() => onOpenChange(false)}
          createLabel="Create season"
          createLoadingLabel="Creating..."
          editLabel="Save changes"
          editLoadingLabel="Saving..."
        />
      </div>
    </EntityDialog>
  );
}
