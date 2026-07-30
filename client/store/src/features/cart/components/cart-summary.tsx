import { Link } from "react-router-dom"

import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"

interface CartSummaryProps {
  subtotal: number
  totalQuantity: number
  canCheckout: boolean
  isAuthenticated: boolean
}

export function CartSummary({
  subtotal,
  totalQuantity,
  canCheckout,
  isAuthenticated,
}: CartSummaryProps) {
  return (
    <Card className="h-fit lg:sticky lg:top-24">
      <CardHeader>
        <CardTitle>Order summary</CardTitle>
      </CardHeader>

      <CardContent className="space-y-4">
        <div className="flex justify-between text-sm">
          <span>Items</span>
          <span>{totalQuantity}</span>
        </div>
        <div className="flex justify-between text-sm">
          <span>Subtotal</span>
          <span>EGP {subtotal.toFixed(2)}</span>
        </div>
        <Separator />
        <p className="text-sm text-muted-foreground">
          Delivery fees are calculated at checkout.
        </p>
      </CardContent>

      <CardFooter>
        {canCheckout ? (
          <Button asChild className="w-full">
            <Link
              to={isAuthenticated ? "/checkout" : "/auth/login"}
              state={!isAuthenticated ? { from: "/checkout" } : undefined}
            >
              {isAuthenticated ? "Proceed to checkout" : "Log in to checkout"}
            </Link>
          </Button>
        ) : (
          <Button className="w-full" disabled>
            Resolve cart warnings first
          </Button>
        )}
      </CardFooter>
    </Card>
  )
}
