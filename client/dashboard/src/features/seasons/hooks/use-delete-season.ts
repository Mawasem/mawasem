import { useMutation, useQueryClient } from "@tanstack/react-query";
import { deleteSeason } from "../api/delete-season";

export function useDeleteSeason() {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: deleteSeason,

		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: ["seasons"],
			});
		},
	});
}
