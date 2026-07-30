import { Navigate, Outlet, useLocation } from "react-router-dom"
import { useCustomerAuthStore } from "../store/use-customer-auth-store"
export function CustomerProtectedRoute() {
  const location = useLocation()
  const status = useCustomerAuthStore((state) => state.status)
  if (status !== "authenticated")
    return (
      <Navigate
        to="/auth/login"
        replace
        state={{ from: location.pathname + location.search }}
      />
    )
  return <Outlet />
}
