import { useMutation, useQueryClient } from "@tanstack/react-query";
import { restoreSeason } from "../api/restore-season";

export function useRestoreSeason() {
	const queryClient = useQueryClient();

	const {
		mutate: restoreSeasonMutation,
		mutateAsync: restoreSeasonMutationAsync,
		isPending: isLoading,
		error,
	} = useMutation({
		mutationFn: restoreSeason,

		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: ["seasons"],
			});
		},
	});

	return {
		restoreSeasonMutation,
		restoreSeasonMutationAsync,
		isLoading,
		error,
	};
}
