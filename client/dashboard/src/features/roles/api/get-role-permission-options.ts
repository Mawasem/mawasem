import { api } from "@/lib/axios";

import type { RolePermissionOptionsResponse } from "../types/role";

export async function getRolePermissionOptions() {
  const response = await api.get<RolePermissionOptionsResponse>(
    "/roles/permission-options"
  );

  return response.data;
}
