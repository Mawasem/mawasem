import {
  LoaderCircle,
  LogIn,
  LogOut,
  Menu,
  Package,
  UserRound,
} from "lucide-react"
import { Link, NavLink } from "react-router-dom"

import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Button, buttonVariants } from "@/components/ui/button"
import {
  Sheet,
  SheetClose,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet"
import { cn } from "@/lib/utils"

import { navbarLinks } from "./navbar-links"
import type { NavbarAccountProps } from "./navbar-types"

export function NavbarMobileNavigation({
  isAuthenticated,
  customerName,
  customerInitials,
  phoneNumber,
  isLoggingOut,
  onLogout,
}: NavbarAccountProps) {
  return (
    <Sheet>
      <SheetTrigger asChild>
        <Button type="button" variant="ghost" size="icon" className="lg:hidden">
          <Menu className="size-5" />
          <span className="sr-only">Open navigation</span>
        </Button>
      </SheetTrigger>

      <SheetContent side="left">
        <SheetHeader className="text-left">
          <SheetTitle className="text-2xl font-bold text-primary">
            Mawasem
          </SheetTitle>
          <SheetDescription>
            Shop products for every season and occasion.
          </SheetDescription>
        </SheetHeader>

        <nav className="mt-8 flex flex-col gap-2">
          {navbarLinks.map((link) => (
            <SheetClose asChild key={link.path}>
              <NavLink
                to={link.path}
                end={link.path === "/"}
                className={({ isActive }) =>
                  cn(
                    buttonVariants({
                      variant: isActive ? "secondary" : "ghost",
                    }),
                    "w-full justify-start"
                  )
                }
              >
                {link.label}
              </NavLink>
            </SheetClose>
          ))}
        </nav>

        <div className="mt-8 space-y-2 border-t pt-6">
          {isAuthenticated ? (
            <>
              <div className="mb-4 flex items-center gap-3 rounded-lg border p-3">
                <Avatar>
                  <AvatarFallback>{customerInitials}</AvatarFallback>
                </Avatar>

                <div className="min-w-0">
                  <p className="truncate text-sm font-medium">{customerName}</p>
                  {phoneNumber ? (
                    <p className="truncate text-xs text-muted-foreground">
                      {phoneNumber}
                    </p>
                  ) : null}
                </div>
              </div>

              <SheetClose asChild>
                <Link
                  to="/account"
                  className={cn(
                    buttonVariants({ variant: "ghost" }),
                    "w-full justify-start"
                  )}
                >
                  <UserRound className="size-4" />
                  My profile
                </Link>
              </SheetClose>

              <SheetClose asChild>
                <Link
                  to="/account/orders"
                  className={cn(
                    buttonVariants({ variant: "ghost" }),
                    "w-full justify-start"
                  )}
                >
                  <Package className="size-4" />
                  My orders
                </Link>
              </SheetClose>

              <SheetClose asChild>
                <Button
                  type="button"
                  variant="destructive"
                  className="w-full justify-start"
                  disabled={isLoggingOut}
                  onClick={() => void onLogout()}
                >
                  {isLoggingOut ? (
                    <LoaderCircle className="size-4 animate-spin" />
                  ) : (
                    <LogOut className="size-4" />
                  )}
                  Log out
                </Button>
              </SheetClose>
            </>
          ) : (
            <SheetClose asChild>
              <Link to="/auth/login" className={cn(buttonVariants(), "w-full")}>
                <LogIn className="size-4" />
                Log in
              </Link>
            </SheetClose>
          )}
        </div>
      </SheetContent>
    </Sheet>
  )
}
