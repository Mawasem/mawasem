import {
  Heart,
  LoaderCircle,
  LogIn,
  LogOut,
  Menu,
  Package,
  Search,
  ShoppingCart,
  UserRound,
} from "lucide-react";
import {
  useState,
  type FormEvent,
} from "react";
import {
  Link,
  NavLink,
  useNavigate,
} from "react-router-dom";

import { ModeToggle } from "@/components/mode-toggle";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import {
  Button,
  buttonVariants,
} from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import {
  Sheet,
  SheetClose,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import { useLogoutCustomer } from "@/features/auth/hooks/use-logout-customer";
import { useCustomerAuthStore } from "@/features/auth/store/use-customer-auth-store";
import { cn } from "@/lib/utils";

const navigationLinks = [
  {
    label: "Home",
    path: "/",
  },
  {
    label: "Products",
    path: "/products",
  },
  {
    label: "Back to School",
    path: "/seasons/back-to-school",
  },
  {
    label: "Summer",
    path: "/seasons/summer",
  },
  {
    label: "Winter",
    path: "/seasons/winter",
  },
];

interface StoreNavbarProps {
  cartCount?: number;
  wishlistCount?: number;
}

export default function StoreNavbar({
  cartCount = 0,
  wishlistCount = 0,
}: StoreNavbarProps) {
  const navigate = useNavigate();

  const [searchQuery, setSearchQuery] =
    useState("");

  const status = useCustomerAuthStore(
    (state) => state.status
  );

  const customer = useCustomerAuthStore(
    (state) => state.customer
  );

  const logoutMutation = useLogoutCustomer();

  const isAuthenticated =
    status === "authenticated";

  const customerName =
    customer?.fullNameEn ||
    customer?.fullNameAr ||
    "Customer";

  const customerInitials = getInitials(
    customerName
  );

  function handleSearch(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    const trimmedQuery = searchQuery.trim();

    if (!trimmedQuery) {
      return;
    }

    navigate(
      `/products?search=${encodeURIComponent(
        trimmedQuery
      )}`
    );
  }

  async function handleLogout() {
    try {
      await logoutMutation.logoutAsync();

      navigate("/", {
        replace: true,
      });
    } catch {
      // The mutation exposes the error state if
      // the application needs to display it.
    }
  }

  return (
    <header className="sticky top-0 z-50 border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/80">
      <div className="container mx-auto flex h-16 items-center gap-4 px-4 lg:px-6">
        {/* Mobile navigation */}
        <Sheet>
          <SheetTrigger asChild>
            <Button
              type="button"
              variant="ghost"
              size="icon"
              className="lg:hidden"
            >
              <Menu className="size-5" />

              <span className="sr-only">
                Open navigation
              </span>
            </Button>
          </SheetTrigger>

          <SheetContent side="left">
            <SheetHeader className="text-left">
              <SheetTitle className="text-2xl font-bold text-primary">
                Mawasem
              </SheetTitle>

              <SheetDescription>
                Shop products for every season
                and occasion.
              </SheetDescription>
            </SheetHeader>

            <nav className="mt-8 flex flex-col gap-2">
              {navigationLinks.map((link) => (
                <SheetClose
                  asChild
                  key={link.path}
                >
                  <NavLink
                    to={link.path}
                    end={link.path === "/"}
                    className={({
                      isActive,
                    }) =>
                      cn(
                        buttonVariants({
                          variant: isActive
                            ? "secondary"
                            : "ghost",
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
                      <AvatarImage
                        src={
                          customer?.profileImageUrl ??
                          undefined
                        }
                        alt={customerName}
                      />

                      <AvatarFallback>
                        {customerInitials}
                      </AvatarFallback>
                    </Avatar>

                    <div className="min-w-0">
                      <p className="truncate text-sm font-medium">
                        {customerName}
                      </p>

                      {customer?.phoneNumber ? (
                        <p className="truncate text-xs text-muted-foreground">
                          {
                            customer.phoneNumber
                          }
                        </p>
                      ) : null}
                    </div>
                  </div>

                  <SheetClose asChild>
                    <Link
                      to="/account"
                      className={cn(
                        buttonVariants({
                          variant: "ghost",
                        }),
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
                        buttonVariants({
                          variant: "ghost",
                        }),
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
                      disabled={
                        logoutMutation.isLoading
                      }
                      onClick={handleLogout}
                    >
                      {logoutMutation.isLoading ? (
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
                  <Link
                    to="/auth/login"
                    className={cn(
                      buttonVariants(),
                      "w-full"
                    )}
                  >
                    <LogIn className="size-4" />
                    Log in
                  </Link>
                </SheetClose>
              )}
            </div>
          </SheetContent>
        </Sheet>

        {/* Logo */}
        <Link
          to="/"
          className="shrink-0 text-2xl font-bold tracking-tight text-primary"
        >
          Mawasem
        </Link>

        {/* Desktop navigation */}
        <nav className="hidden items-center gap-1 lg:flex">
          {navigationLinks.map((link) => (
            <NavLink
              key={link.path}
              to={link.path}
              end={link.path === "/"}
              className={({ isActive }) =>
                cn(
                  buttonVariants({
                    variant: isActive
                      ? "secondary"
                      : "ghost",
                    size: "sm",
                  }),
                  !isActive &&
                  "text-muted-foreground"
                )
              }
            >
              {link.label}
            </NavLink>
          ))}
        </nav>

        {/* Search */}
        <form
          onSubmit={handleSearch}
          className="relative mx-auto hidden max-w-md flex-1 md:block"
        >
          <Search className="absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" />

          <Input
            type="search"
            value={searchQuery}
            onChange={(event) =>
              setSearchQuery(
                event.target.value
              )
            }
            placeholder="Search for products..."
            className="pl-9"
          />
        </form>

        {/* Navbar actions */}
        <div className="ml-auto flex items-center gap-1">
          {/* Mobile search */}
          <Button
            type="button"
            variant="ghost"
            size="icon"
            className="md:hidden"
            onClick={() =>
              navigate("/products")
            }
          >
            <Search className="size-5" />

            <span className="sr-only">
              Search
            </span>
          </Button>

          <ModeToggle />

          {/* Wishlist */}
          <Button
            variant="ghost"
            size="icon"
            asChild
            className="relative"
          >
            <Link to="/wishlist">
              <Heart className="size-5" />

              {wishlistCount > 0 ? (
                <Badge className="absolute -top-1 -right-1 flex size-5 items-center justify-center rounded-full p-0 text-[10px]">
                  {wishlistCount > 99
                    ? "99+"
                    : wishlistCount}
                </Badge>
              ) : null}

              <span className="sr-only">
                Wishlist
              </span>
            </Link>
          </Button>

          {/* Shopping cart */}
          <Button
            variant="ghost"
            size="icon"
            asChild
            className="relative"
          >
            <Link to="/cart">
              <ShoppingCart className="size-5" />

              {cartCount > 0 ? (
                <Badge className="absolute -top-1 -right-1 flex size-5 items-center justify-center rounded-full p-0 text-[10px]">
                  {cartCount > 99
                    ? "99+"
                    : cartCount}
                </Badge>
              ) : null}

              <span className="sr-only">
                Shopping cart
              </span>
            </Link>
          </Button>

          {/* Authentication action */}
          {isAuthenticated ? (
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
                    <AvatarImage
                      src={
                        customer
                          ?.profileImageUrl ??
                        undefined
                      }
                      alt={customerName}
                    />

                    <AvatarFallback className="text-xs">
                      {customerInitials}
                    </AvatarFallback>
                  </Avatar>
                </Button>
              </DropdownMenuTrigger>

              <DropdownMenuContent
                align="end"
                className="w-56"
              >
                <DropdownMenuLabel>
                  <div className="flex flex-col gap-1">
                    <span className="truncate">
                      {customerName}
                    </span>

                    {customer?.phoneNumber ? (
                      <span className="truncate text-xs font-normal text-muted-foreground">
                        {
                          customer.phoneNumber
                        }
                      </span>
                    ) : null}
                  </div>
                </DropdownMenuLabel>

                <DropdownMenuSeparator />

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

                <DropdownMenuSeparator />

                <DropdownMenuItem
                  variant="destructive"
                  disabled={
                    logoutMutation.isLoading
                  }
                  onSelect={(event) => {
                    event.preventDefault();
                    void handleLogout();
                  }}
                >
                  {logoutMutation.isLoading ? (
                    <LoaderCircle className="size-4 animate-spin" />
                  ) : (
                    <LogOut className="size-4" />
                  )}

                  {logoutMutation.isLoading
                    ? "Logging out..."
                    : "Log out"}
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          ) : (
            <Button
              variant="outline"
              size="sm"
              asChild
              className="hidden sm:flex"
            >
              <Link to="/auth/login">
                <UserRound className="size-4" />
                Log in
              </Link>
            </Button>
          )}
        </div>
      </div>
    </header>
  );
}

function getInitials(name: string) {
  const words = name
    .trim()
    .split(/\s+/)
    .filter(Boolean);

  if (words.length === 0) {
    return "CU";
  }

  return words
    .slice(0, 2)
    .map((word) => word.charAt(0))
    .join("")
    .toUpperCase();
}