import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateSeason } from "../api/update-season";

export function useUpdateSeason() {
	const queryClient = useQueryClient();

	const {
		mutate: updateSeasonMutation,
		mutateAsync: updateSeasonMutationAsync,
		error,
		isPending: isLoading
	} = useMutation({
		mutationFn: updateSeason,

		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: ["seasons"],
			});
		},
	});

	return {
		updateSeasonMutation,
		updateSeasonMutationAsync,
		error,
		isLoading
	}
}
