import { Search, ShoppingCart, UserRound } from "lucide-react"
import { Link, useNavigate } from "react-router-dom"

import { ModeToggle } from "@/components/mode-toggle"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"

import { NavbarAccountMenu } from "./navbar-account-menu"
import type { NavbarAccountProps } from "./navbar-types"

interface NavbarActionsProps extends NavbarAccountProps {
  cartCount: number
  wishlistCount: number
}

export function NavbarActions({
  cartCount,
  isAuthenticated,
  ...accountProps
}: NavbarActionsProps) {
  const navigate = useNavigate()

  return (
    <div className="ml-auto flex items-center gap-1">
      <Button
        type="button"
        variant="ghost"
        size="icon"
        className="md:hidden"
        onClick={() => navigate("/products")}
      >
        <Search className="size-5" />
        <span className="sr-only">Search</span>
      </Button>

      <ModeToggle />

      <NavbarIconLink
        to="/cart"
        label="Shopping cart"
        count={cartCount}
        icon={<ShoppingCart className="size-5" />}
      />

      {isAuthenticated ? (
        <NavbarAccountMenu {...accountProps} />
      ) : (
        <Button variant="outline" size="sm" asChild className="hidden sm:flex">
          <Link to="/auth/login">
            <UserRound className="size-4" />
            Log in
          </Link>
        </Button>
      )}
    </div>
  )
}

interface NavbarIconLinkProps {
  to: string
  label: string
  count: number
  icon: React.ReactNode
}

function NavbarIconLink({ to, label, count, icon }: NavbarIconLinkProps) {
  return (
    <Button variant="ghost" size="icon" asChild className="relative">
      <Link to={to}>
        {icon}
        {count > 0 ? (
          <Badge className="absolute -top-1 -right-1 flex size-5 items-center justify-center rounded-full p-0 text-[10px]">
            {count > 99 ? "99+" : count}
          </Badge>
        ) : null}
        <span className="sr-only">{label}</span>
      </Link>
    </Button>
  )
}
