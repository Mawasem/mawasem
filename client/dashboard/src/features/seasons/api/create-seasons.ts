import { api } from "@/lib/axios";
import type { Season, SeasonPayload } from "../types";

export async function createSeason(
	data: SeasonPayload
) {
	const response = await api.post<Season>(
		"/seasons",
		data
	);

	return response.data;
}
