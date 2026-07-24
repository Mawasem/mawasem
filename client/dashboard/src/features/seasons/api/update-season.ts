import { api } from "@/lib/axios";

export async function updateSeason({
	seasonId,
	data,
}: {
	seasonId: number;
	data: unknown;
}) {
	const response = await api.put(
		`/admin/seasons/${seasonId}`,
		data
	);

	return response.data;
}
