import { EntityDialog } from "@/components/entity-dialog/entity-dialog";
import { EntityDialogFooter } from "@/components/entity-dialog/entity-dialog-footer";
import { useTranslation } from "react-i18next";

import { useCreateCategory } from "../hooks/use-create-category";
import { useUpdateCategory } from "../hooks/use-update-category";
import type { CategoryFormValues } from "../schema/category-form-schema";
import { CategoryForm } from "./category-form";
import type { CategoryDialogProps } from "./types";

export function CategoryDialog({
  open,
  onOpenChange,
  mode,
  category,
}: CategoryDialogProps) {
  const { t } = useTranslation();

  const createCategoryMutation = useCreateCategory();
  const updateCategoryMutation = useUpdateCategory();

  const isEditMode = mode === "edit";

  const title = isEditMode
    ? t("categories.dialog.editTitle")
    : t("categories.dialog.createTitle");

  const description = isEditMode
    ? t("categories.dialog.editDescription")
    : t("categories.dialog.createDescription");

  const formId = `category-form-${mode}`;

  const isSubmitting =
    createCategoryMutation.isLoading ||
    updateCategoryMutation.isLoading;

  const mutationError =
    createCategoryMutation.error ??
    updateCategoryMutation.error;

  const errorMessage =
    mutationError instanceof Error
      ? mutationError.message
      : null;

  const handleSubmit = async (
    values: CategoryFormValues
  ) => {
    try {
      if (isEditMode && category) {
        await updateCategoryMutation.updateCategoryMutationAsync({
          id: category.id,
          data: values,
        });
      } else {
        await createCategoryMutation.createCategoryMutationAsync(
          values
        );
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
        <CategoryForm
          mode={mode}
          category={category}
          formId={formId}
          onSubmit={handleSubmit}
          errorMessage={errorMessage}
        />

        <EntityDialogFooter
          mode={mode}
          formId={formId}
          isLoading={isSubmitting}
          onCancel={() => onOpenChange(false)}
          createLabel={t("categories.actions.create")}
          createLoadingLabel={t("common.creating")}
          editLabel={t("common.saveChanges")}
          editLoadingLabel={t("common.saving")}
        />
      </div>
    </EntityDialog>
  );
}
