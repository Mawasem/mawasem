// features/brands/components/brand-columns.tsx
import type { ColumnDef } from "@tanstack/react-table";

import { Badge } from "@/components/ui/badge";
import { BrandActions } from "@/features/brands/components/brand-actions";
import type { Brand } from "@/features/brands/types/brand";


export const brandColumns: ColumnDef<Brand>[] = [
  {
    accessorKey: "nameAr",
    header: "Arabic Name",
  },

  {
    accessorKey: "nameEn",
    header: "English Name",
  },

  {
    accessorKey: "descriptionAr",
    header: "Arabic Description",

    cell: ({ row }) => {
      const description = row.original.descriptionAr;

      return (
        <span className="line-clamp-1 max-w-[250px]">
          {description}
        </span>
      );
    },
  },

  {
    accessorKey: "isActive",
    header: "Status",

    cell: ({ row }) => {
      const isActive = row.original.isActive;
      const isDeleted = Boolean(
        (row.original as { isDeleted?: boolean; IsDeleted?: boolean }).isDeleted ??
          (row.original as { isDeleted?: boolean; IsDeleted?: boolean }).IsDeleted
      );

      return (
        <Badge
          variant={isDeleted ? "secondary" : isActive ? "default" : "secondary"}
        >
          {isDeleted ? "Deleted" : isActive ? "Active" : "Inactive"}
        </Badge>
      );
    },
  },

  {
    id: "actions",
    header: "",

    cell: ({ row }) => (
      <div className="flex justify-end">
        <BrandActions
          brand={row.original}
        />
      </div>
    ),
  },
];