import {
  Building2,
  FolderKanban,
  LayoutGrid,
  Leaf,
  MapPin,
  PackageSearch,
  ShieldCheck,
  Users,
  UsersRound
} from "lucide-react";

export const data = [
  {
    key: "catalog",
    url: "#",

    items: [
      {
        key: "brands",
        url: "/brands",
        icon: Building2,
      },
      {
        key: "categories",
        url: "/categories",
        icon: LayoutGrid,
      },
      {
        key: "collections",
        url: "/collections",
        icon: FolderKanban,
      },
      {
        key: "seasons",
        url: "/seasons",
        icon: Leaf,
      },
      {
        key: "products",
        url: "/products",
        icon: PackageSearch,
      },
    ],
  },
  {
    key: "operations",
    url: "#",

    items: [
      {
        key: "deliveryAreas",
        url: "/delivery-areas",
        icon: MapPin,
      },
    ],
  },
  {
    key: "customers",
    url: "#",

    items: [
      {
        key: "customers",
        url: "/customers",
        icon: Users,
      },
      {
        key: "roles",
        url: "/roles",
        icon: ShieldCheck,
      },
      {
        key: "employees",
        url: "/employees",
        icon: UsersRound,
      },
    ],
  },
];