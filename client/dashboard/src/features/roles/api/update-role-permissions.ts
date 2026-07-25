import { api } from "@/lib/axios";

import type { Role, UpdateRolePermissionsRequest } from "../types/role";

export async function updateRolePermissions({
  roleName,
  permissionNames,
}: {
  roleName: string;
  permissionNames: string[];
}) {
  const response = await api.put<Role>(
    `/roles/${encodeURIComponent(roleName)}/permissions`,
    {
      permissionNames,
    } satisfies UpdateRolePermissionsRequest
  );

  return response.data;
}
