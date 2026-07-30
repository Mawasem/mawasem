import { LoaderCircle, ShoppingBag, Trash2 } from "lucide-react"
import { Link } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { getApiErrorMessage } from "@/lib/get-api-error-message"
import { CartItemCard } from "../components/cart-item-card"
import { CartSummary } from "../components/cart-summary"
import { useCart } from "../hooks/use-cart"
import { useClearCart } from "../hooks/use-clear-cart"
import { useRemoveCartItem } from "../hooks/use-remove-cart-item"
import { useUpdateCartItem } from "../hooks/use-update-cart-item"

export default function CartPage() {
  const { cartData, isLoading, error, isAuthenticated } = useCart()
  const updateMutation = useUpdateCartItem()
  const removeMutation = useRemoveCartItem()
  const clearMutation = useClearCart()
  const busy =
    updateMutation.isLoading ||
    removeMutation.isLoading ||
    clearMutation.isLoading

  if (isLoading)
    return (
      <div className="grid min-h-64 place-items-center">
        <LoaderCircle className="size-8 animate-spin" />
      </div>
    )
  if (error)
    return (
      <p className="rounded-lg border border-destructive/30 bg-destructive/5 p-4 text-destructive">
        {getApiErrorMessage(error, "Could not load your cart.")}
      </p>
    )
  if (!cartData || cartData.items.length === 0) {
    return (
      <div className="grid min-h-[50vh] place-items-center text-center">
        <div className="space-y-4">
          <ShoppingBag className="mx-auto size-12 text-muted-foreground" />
          <h1 className="text-2xl font-bold">Your cart is empty</h1>
          <Button asChild>
            <Link to="/seasons/back-to-school">Continue shopping</Link>
          </Button>
        </div>
      </div>
    )
  }

  return (
    <section className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Shopping cart</h1>
          <p className="text-muted-foreground">
            Review your products before checkout.
          </p>
        </div>
        <Button
          variant="outline"
          disabled={busy}
          onClick={() => void clearMutation.clearCartAsync()}
        >
          {clearMutation.isLoading ? (
            <LoaderCircle className="size-4 animate-spin" />
          ) : (
            <Trash2 className="size-4" />
          )}{" "}
          Clear cart
        </Button>
      </div>
      <div className="grid gap-6 lg:grid-cols-[minmax(0,1fr)_22rem]">
        <div className="space-y-4">
          {cartData.items.map((item) => (
            <CartItemCard
              key={item.cartItemId}
              item={item}
              busy={busy}
              onQuantityChange={(quantity) =>
                void updateMutation.updateCartItemAsync({
                  cartItemId: item.cartItemId,
                  quantity,
                })
              }
              onRemove={() =>
                void removeMutation.removeCartItemAsync(item.cartItemId)
              }
            />
          ))}
        </div>
        <CartSummary
          subtotal={cartData.subtotal}
          totalQuantity={cartData.totalQuantity}
          canCheckout={!cartData.hasWarnings}
          isAuthenticated={isAuthenticated}
        />
      </div>
    </section>
  )
}
