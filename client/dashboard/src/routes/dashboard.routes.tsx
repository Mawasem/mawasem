import { BrandsPage } from "@/features/brands/pages/BrandsPage";
import CategoriesPage from "@/features/categories/pages/CategoriesPage";
import CollectionsPage from "@/features/collections/pages/CollectionsPage";
import CustomerPage from "@/features/customers/pages/CustomerPage";
import RolesPage from "@/features/roles/pages/RolesPage";
import SeasonsPage from "@/features/seasons/pages/SeasonsPage";
import AdminLayout from "@/layouts/AdminLayout";
import DashboardPage from "@/pages/Home/DashboardPage";
import type { RouteObject } from "react-router-dom";
import { ProtectedRoute } from "./protected-route";

export const dashboardRoutes: RouteObject = {
  path: "/",
  element: (
    <ProtectedRoute>
      <AdminLayout />
    </ProtectedRoute>
  ),
  children: [
    {
      index: true,
      element: <DashboardPage />,
    },
    {
      path: "brands",
      element: <BrandsPage />
    },
    {
      path: 'categories',
      element: <CategoriesPage />
    },
    {
      path: "collections",
      element: <CollectionsPage />
    },
    {
      path: "seasons",
      element: <SeasonsPage />
    },
    {
      path: "customers",
      element: <CustomerPage />
    },
    {
      path: "roles",
      element: <RolesPage />
    }
  ],
};
