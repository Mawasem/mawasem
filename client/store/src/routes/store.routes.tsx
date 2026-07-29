import StoreLayout from "@/layouts/StoreLayout";
import HomePage from "@/features/home/pages/HomePage";
import type { RouteObject } from "react-router-dom";

export const storeRoutes: RouteObject = {
  path: "/",
  element: <StoreLayout />,
  children: [
    {
      index: true,
      element: <HomePage />
    }

  ]
}
