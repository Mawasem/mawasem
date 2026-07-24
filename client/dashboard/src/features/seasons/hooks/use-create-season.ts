import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createSeason } from "../api/create-seasons";

export function useCreateSeason() {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: createSeason,

		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: ["seasons"],
			});
		},
	});
}
