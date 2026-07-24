import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createSeason } from "../api/create-seasons";

export function useCreateSeason() {
	const queryClient = useQueryClient();
	
	const {
		mutate: createSeasonMutation,
		mutateAsync: createSeasonMutationAsync,
		isPending: isLoading,
		error
	} = useMutation({
		mutationFn: createSeason,

		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: ["seasons"],
			});
		},
	});

	return {
		createSeasonMutation,
		createSeasonMutationAsync,
		isLoading,
		error
	}
}
