import { api } from "@/lib/axios";

export async function restoreSeason(
	seasonId: number
) {
	const response =
		await api.post(
			`/admin/seasons/${seasonId}/restore`
		);

	return response.data;
}
