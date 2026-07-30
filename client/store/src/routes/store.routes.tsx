import type { RouteObject } from "react-router-dom"

import BackToSchoolPage from "@/features/catalog/pages/BackToSchoolPage"
import ProductDetailsPage from "@/features/catalog/pages/ProductDetailsPage"
import HomePage from "@/features/home/pages/HomePage"
import CartPage from "@/features/cart/pages/CartPage"
import CheckoutPage from "@/features/checkout/pages/CheckoutPage"
import CheckoutSuccessPage from "@/features/checkout/pages/CheckoutSuccessPage"
import { CustomerProtectedRoute } from "@/features/auth/components/customer-protected-route"
import StoreLayout from "@/layouts/StoreLayout"

export const storeRoutes: RouteObject = {
  path: "/",
  element: <StoreLayout />,
  children: [
    {
      index: true,
      element: <HomePage />,
    },
    {
      path: "seasons/back-to-school",
      element: <BackToSchoolPage />,
    },
    {
      path: "products/:slug",
      element: <ProductDetailsPage />,
    },
    {
      path: "cart",
      element: <CartPage />,
    },
    {
      element: <CustomerProtectedRoute />,
      children: [
        { path: "checkout", element: <CheckoutPage /> },
        {
          path: "checkout/success/:orderNumber",
          element: <CheckoutSuccessPage />,
        },
      ],
    },
  ],
}
