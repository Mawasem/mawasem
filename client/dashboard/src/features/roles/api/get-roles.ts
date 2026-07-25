import { api } from "@/lib/axios";

import type { RoleListResponse } from "../types/role";

export async function getRoles() {
  const response = await api.get<RoleListResponse>("/roles");

  return response.data;
}
