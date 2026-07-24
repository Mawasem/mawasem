import { useMutation, useQueryClient } from "@tanstack/react-query";
import { deleteSeason } from "../api/delete-season";

export function useDeleteSeason() {
	const queryClient = useQueryClient();

	const {
		mutate: deleteSeasonMutation,
		mutateAsync: deleteSeasonMutationAsync,
		isPending: isLoading,
		error
	} = useMutation({
		mutationFn: deleteSeason,
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: ["seasons"],
			});
		},
	});

	return {
		deleteSeasonMutation,
		deleteSeasonMutationAsync,
		isLoading,
		error
	}
}
