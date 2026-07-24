import { api } from "@/lib/axios";
import type { PaginatedResponse } from "@/types/pagination";
import type { Season, SeasonQueryParams } from "../types";

export async function getSeasons({
	search,
	isActive,
	includeDeleted,
	pageNumber,
	pageSize,
}: SeasonQueryParams) {
	const response =
		await api.get<
			PaginatedResponse<Season>
		>(
			"/seasons",
			{
				params: {
					search,
					isActive,
					includeDeleted,
					pageNumber,
					pageSize,
				},
			}
		);

	return response.data;
}
