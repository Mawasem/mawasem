import { useQuery } from "@tanstack/react-query";
import { getSeasonById } from "../api/get-season";

export function useSeason(id: number) {
	const {
		data: season,
		isLoading,
		error,
	} = useQuery({
		queryKey: ["season", id],

		queryFn: () => getSeasonById(id),

		enabled: !!id,
	});

	return {
		season,
		isLoading,
		error,
	};
}
