export interface Role {
  name: string;
  isProtected: boolean;
  canManagePermissions: boolean;
  assignedUserCount: number;
  permissionNames: string[];
}

export interface RoleListResponse {
  items: Role[];
}

export interface PermissionOption {
  name: string;
  description: string;
  isRequired: boolean;
}

export interface RolePermissionOptionsResponse {
  items: PermissionOption[];
}

export interface UpdateRolePermissionsRequest {
  permissionNames: string[];
}
