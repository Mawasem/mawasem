import { EntityDialog } from "@/components/entity-dialog/entity-dialog";
import { EntityDialogFooter } from "@/components/entity-dialog/entity-dialog-footer";
import { useTranslation } from "react-i18next";

import { useCreateCollection } from "../hooks/use-create-collection";
import { useUpdateCollection } from "../hooks/use-update-collection";
import type { CollectionFormValues } from "../schema/collection-form-schema";
import { CollectionForm } from "./collection-form";
import type { CollectionDialogProps } from "./types";

export function CollectionDialog({
	open,
	onOpenChange,
	mode,
	collection,
}: CollectionDialogProps) {
	const { t } = useTranslation();

	const createCollectionMutation =
		useCreateCollection();
	const updateCollectionMutation =
		useUpdateCollection();

	const isEditMode = mode === "edit";

	const title = isEditMode
		? t("collections.dialog.editTitle")
		: t("collections.dialog.createTitle");

	const description = isEditMode
		? t("collections.dialog.editDescription")
		: t("collections.dialog.createDescription");

	const formId = `collection-form-${mode}`;

	const isSubmitting =
		createCollectionMutation.isLoading ||
		updateCollectionMutation.isLoading;

	const mutationError =
		createCollectionMutation.error ??
		updateCollectionMutation.error;

	const errorMessage =
		mutationError instanceof Error
			? mutationError.message
			: null;

	const handleSubmit = async (
		values: CollectionFormValues
	) => {
		try {
			if (isEditMode && collection) {
				await updateCollectionMutation.updateCollectionAsync({
					id: collection.id,
					data: values,
				});
			} else {
				await createCollectionMutation.createCollectionAsync(
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
				<CollectionForm
					mode={mode}
					collection={collection}
					formId={formId}
					onSubmit={handleSubmit}
					errorMessage={errorMessage}
				/>

				<EntityDialogFooter
					mode={mode}
					formId={formId}
					isLoading={isSubmitting}
					onCancel={() => onOpenChange(false)}
					createLabel={t("collections.actions.create")}
					createLoadingLabel={t("common.creating")}
					editLabel={t("common.saveChanges")}
					editLoadingLabel={t("common.saving")}
				/>
			</div>
		</EntityDialog>
	);
}
