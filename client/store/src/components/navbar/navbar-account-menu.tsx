import { LoaderCircle, LogOut, Package, UserRound } from "lucide-react"
import { Link } from "react-router-dom"

import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"

import type { NavbarAccountProps } from "./navbar-types"

type NavbarAccountMenuProps = Omit<NavbarAccountProps, "isAuthenticated">

export function NavbarAccountMenu({
  customerName,
  customerInitials,
  phoneNumber,
  isLoggingOut,
  onLogout,
}: NavbarAccountMenuProps) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="rounded-full"
          aria-label="Open account menu"
        >
          <Avatar className="size-8">
            <AvatarFallback className="text-xs">
              {customerInitials}
            </AvatarFallback>
          </Avatar>
        </Button>
      </DropdownMenuTrigger>

      <DropdownMenuContent align="end" className="w-56">
        <div className="px-2 py-1.5 text-sm font-medium">
          <div className="flex flex-col gap-1">
            <span className="truncate">{customerName}</span>
            {phoneNumber ? (
              <span className="truncate text-xs font-normal text-muted-foreground">
                {phoneNumber}
              </span>
            ) : null}
          </div>
        </div>

        <div role="separator" className="-mx-1 my-1 h-px bg-border" />

        <DropdownMenuItem asChild>
          <Link to="/account">
            <UserRound className="size-4" />
            My profile
          </Link>
        </DropdownMenuItem>

        <DropdownMenuItem asChild>
          <Link to="/account/orders">
            <Package className="size-4" />
            My orders
          </Link>
        </DropdownMenuItem>

        <div role="separator" className="-mx-1 my-1 h-px bg-border" />

        <DropdownMenuItem
          className="text-destructive focus:bg-destructive/10 focus:text-destructive"
          disabled={isLoggingOut}
          onSelect={(event) => {
            event.preventDefault()
            void onLogout()
          }}
        >
          {isLoggingOut ? (
            <LoaderCircle className="size-4 animate-spin" />
          ) : (
            <LogOut className="size-4" />
          )}
          {isLoggingOut ? "Logging out..." : "Log out"}
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  )
}
