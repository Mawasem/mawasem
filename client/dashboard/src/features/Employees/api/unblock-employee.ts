import { api } from "@/lib/axios"

export async function unblockEmployee(employeeId: number) {
  await api.post(`/employees/${employeeId}/unblock`)
}
