import { api } from "@/lib/axios";
import type { Season } from "../types";

export async function getSeasonById(
	id: number
) {
	const response = await api.get<Season>(
		`/seasons/${id}`
	);

	return response.data;
}
