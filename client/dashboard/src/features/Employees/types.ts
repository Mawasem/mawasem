import type { PaginatedResponse } from "@/types/pagination"

export interface Employee {
  id: number
  fullNameAr: string
  fullNameEn: string
  email: string
  isBlocked: boolean
  blockedAt: string | null
  blockedReason: string | null
  mustChangePassword: boolean
  roles: string[]
  directPermissions: string[]
  effectivePermissions: string[]
}

export interface GetEmployeesParams {
  search?: string
  isBlocked?: boolean
  pageNumber: number
  pageSize: number
}

export type EmployeesResponse = PaginatedResponse<Employee>

export interface EmployeeAccessOptions {
  roleNames: string[]
  permissionNames: string[]
}

export interface EmployeeActionsProps {
  employee: Employee
}

export type EmployeeDialogMode = "create" | "edit"

export interface EmployeeDialogStateProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

export interface EmployeeDialogProps extends EmployeeDialogStateProps {
  mode: EmployeeDialogMode
  employee?: Employee
}

export interface UnblockEmployeeDialogProps extends EmployeeDialogStateProps {
  employee: Employee
}

export interface BlockEmployeeRequest {
  reason: string
}

export interface BlockEmployeeParams {
  employeeId: number
  data: BlockEmployeeRequest
}

export interface BlockEmployeeDialogProps extends EmployeeDialogStateProps {
  employee: Employee
}

export interface EmployeeDetailsDialogProps extends EmployeeDialogStateProps {
  employee: Employee
}

export interface UpdateEmployeeRequest {
  fullNameAr: string
  fullNameEn: string
  email: string
}

export interface CreateEmployeeRequest extends UpdateEmployeeRequest {
  temporaryPassword: string
  confirmTemporaryPassword: string
  roleNames: string[]
  permissionNames: string[]
}

export interface UpdateEmployeeParams {
  employeeId: number
  data: UpdateEmployeeRequest
}

export interface UpdateEmployeePermissionsRequest {
  permissionNames: string[]
}

export interface UpdateEmployeePermissionsParams {
  employeeId: number
  data: UpdateEmployeePermissionsRequest
}

export interface ManageEmployeePermissionsDialogProps extends EmployeeDialogStateProps {
  employee: Employee
}

export interface UpdateEmployeeRolesRequest {
  roleNames: string[]
}

export interface UpdateEmployeeRolesParams {
  employeeId: number
  data: UpdateEmployeeRolesRequest
}

export interface ManageEmployeeRolesDialogProps extends EmployeeDialogStateProps {
  employee: Employee
}

export interface ResetEmployeePasswordRequest {
  temporaryPassword: string
  confirmTemporaryPassword: string
}

export interface ResetEmployeePasswordParams {
  employeeId: number
  data: ResetEmployeePasswordRequest
}

export interface ResetEmployeePasswordDialogProps extends EmployeeDialogStateProps {
  employee: Employee
}
