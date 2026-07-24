import { api } from "@/lib/axios";

export async function getSeasonById(
	seasonId: number
) {
	const response = await api.get(
		`/admin/seasons/${seasonId}`
	);

	return response.data;
}
