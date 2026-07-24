import { api } from "@/lib/axios";
import type { Season, UpdateSeasonParams } from "../types";

export async function updateSeason({
	id,
	data,
}: UpdateSeasonParams) {
	const response = await api.put<Season>(
		`/seasons/${id}`,
		data
	);

	return response.data;
}
