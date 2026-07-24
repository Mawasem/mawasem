import { api } from "@/lib/axios";

export async function createSeason(
	data: unknown
) {
	const response = await api.post(
		"/admin/seasons",
		data
	);

	return response.data;
}
