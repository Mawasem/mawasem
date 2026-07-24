import { useQuery } from "@tanstack/react-query";
import { getSeasons } from "../api/get-seasons";

export function useSeasons(
	params: Parameters<typeof getSeasons>[0]
) {
	const {
		data,
		isPending: isLoading,
		error,
	} = useQuery({
		queryKey: ["seasons", params],

		queryFn: () => getSeasons(params),
	});

	return {
		data,
		isLoading,
		error,
	};
}
