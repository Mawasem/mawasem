import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateSeason } from "../api/update-season";

export function useUpdateSeason() {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: updateSeason,

		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: ["seasons"],
			});
		},
	});
}
