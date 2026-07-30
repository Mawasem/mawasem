import { useEffect, useRef, type PropsWithChildren } from "react"
import { LoaderCircle } from "lucide-react"
import { refreshCustomerSession } from "../api/refresh-customer-session"
import { useCustomerAuthStore } from "../store/use-customer-auth-store"

export function CustomerAuthBootstrap({ children }: PropsWithChildren) {
  const started = useRef(false)
  const status = useCustomerAuthStore((state) => state.status)

  useEffect(() => {
    if (started.current) return
    started.current = true
    void refreshCustomerSession()
      .then(useCustomerAuthStore.getState().setSession)
      .catch(useCustomerAuthStore.getState().clearSession)
  }, [])

  if (status === "checking") {
    return (
      <div className="grid min-h-svh place-items-center">
        <LoaderCircle className="size-6 animate-spin" />
      </div>
    )
  }

  return children
}
