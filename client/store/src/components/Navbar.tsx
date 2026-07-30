import { Link, useNavigate } from "react-router-dom"

import { useLogoutCustomer } from "@/features/auth/hooks/use-logout-customer"
import { useCustomerAuthStore } from "@/features/auth/store/use-customer-auth-store"
import { useCart } from "@/features/cart/hooks/use-cart"

import { NavbarActions } from "./navbar/navbar-actions"
import { NavbarDesktopNavigation } from "./navbar/navbar-desktop-navigation"
import { NavbarMobileNavigation } from "./navbar/navbar-mobile-navigation"
import { getInitials } from "./navbar/navbar-utils"

interface StoreNavbarProps {
  cartCount?: number
  wishlistCount?: number
}

export default function StoreNavbar({
  cartCount = 0,
  wishlistCount = 0,
}: StoreNavbarProps) {
  const navigate = useNavigate()
  const status = useCustomerAuthStore((state) => state.status)
  const customer = useCustomerAuthStore((state) => state.user)
  const logoutMutation = useLogoutCustomer()
  const { cartData } = useCart()

  const isAuthenticated = status === "authenticated"
  const customerName =
    customer?.fullNameEn || customer?.fullNameAr || "Customer"
  const customerInitials = getInitials(customerName)

  const accountProps = {
    isAuthenticated,
    customerName,
    customerInitials,
    phoneNumber: customer?.phoneNumber ?? undefined,
    isLoggingOut: logoutMutation.isLoading,
    onLogout: handleLogout,
  }

  async function handleLogout() {
    try {
      await logoutMutation.logoutAsync()
      navigate("/auth/login", { replace: true })
    } catch {
      // The mutation retains the error state for the calling UI.
    }
  }

  return (
    <header className="sticky top-0 z-50 border-b bg-background/95 backdrop-blur">
      <div className="container mx-auto flex h-16 items-center gap-4 px-4 lg:px-6">
        <NavbarMobileNavigation {...accountProps} />

        <Link
          to="/"
          className="shrink-0 text-2xl font-bold tracking-tight text-primary"
        >
          Mawasem
        </Link>

        <NavbarDesktopNavigation />

        <NavbarActions
          {...accountProps}
          cartCount={cartData?.totalQuantity ?? cartCount}
          wishlistCount={wishlistCount}
        />
      </div>
    </header>
  )
}
