import { api } from "@/lib/axios";

export async function getSeasons({
	search,
	isActive,
	includeDeleted,
	pageNumber,
	pageSize,
}: {
	search?: string;
	isActive?: boolean;
	includeDeleted?: boolean;
	pageNumber: number;
	pageSize: number;
}) {
	const response =
		await api.get(
			"/admin/seasons",
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
