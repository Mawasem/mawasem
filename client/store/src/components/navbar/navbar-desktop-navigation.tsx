import { NavLink } from "react-router-dom"

import { buttonVariants } from "@/components/ui/button"
import { cn } from "@/lib/utils"

import { navbarLinks } from "./navbar-links"

export function NavbarDesktopNavigation() {
  return (
    <nav className="hidden items-center gap-1 lg:flex">
      {navbarLinks.map((link) => (
        <NavLink
          key={link.path}
          to={link.path}
          end={link.path === "/"}
          className={({ isActive }) =>
            cn(
              buttonVariants({
                variant: isActive ? "secondary" : "ghost",
                size: "sm",
              }),
              !isActive && "text-muted-foreground"
            )
          }
        >
          {link.label}
        </NavLink>
      ))}
    </nav>
  )
}
