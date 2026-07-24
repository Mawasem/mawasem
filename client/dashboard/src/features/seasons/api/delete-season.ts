import { api } from "@/lib/axios";

export async function deleteSeason(
	seasonId: number
) {
	await api.delete(
		`/seasons/${seasonId}`
	);
}
