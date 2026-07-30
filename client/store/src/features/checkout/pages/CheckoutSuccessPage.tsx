import { CheckCircle2 } from "lucide-react"
import { Link, useLocation, useParams } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import type { PlaceOrderResponse } from "../types/checkout.types"
export default function CheckoutSuccessPage() {
  const { orderNumber } = useParams()
  const location = useLocation()
  const order = (location.state as { order?: PlaceOrderResponse } | null)?.order
  return (
    <div className="mx-auto max-w-xl py-12">
      <Card>
        <CardContent className="space-y-6 p-8 text-center">
          <CheckCircle2 className="mx-auto size-16 text-primary" />
          <div>
            <h1 className="text-3xl font-bold">Order placed successfully</h1>
            <p className="mt-2 text-muted-foreground">
              Your order number is <strong>{orderNumber}</strong>.
            </p>
          </div>
          {order ? (
            <div className="rounded-lg bg-muted p-4">
              <p className="text-sm text-muted-foreground">Total</p>
              <p className="text-2xl font-bold">
                EGP {order.totalAmount.toFixed(2)}
              </p>
            </div>
          ) : null}
          <div className="flex justify-center gap-3">
            <Button asChild>
              <Link to="/">Continue shopping</Link>
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
