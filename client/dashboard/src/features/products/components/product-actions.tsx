import { MoreHorizontal } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import type { ProductActionsProps } from "../types";
import { ProductDeleteDialog } from "./product-delete-dialog";
import { ProductDetailsDialog } from "./product-details-dialog";
import { ProductDialog } from "./product-dialog";
import { ProductImagesDialog } from "./product-images-dialog";
import { ProductRestoreDialog } from "./product-restore-dialog";
import { ProductStatusDialog } from "./product-status-dialog";
import { ProductVariantsDialog } from "./product-variants-dialog";

export function ProductActions({ product }: ProductActionsProps) {
  const { t } = useTranslation();
  const [dialog, setDialog] = useState<
    "details" | "edit" | "status" | "variants" | "images" | "delete" | "restore" | null
  >(null);

  const closeDialog = () => setDialog(null);

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            size="icon-sm"
            aria-label={t("products.actions.openActions")}
          >
            <MoreHorizontal className="size-4" />
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
          <DropdownMenuItem onClick={() => setDialog("details")}>
            {t("products.actions.viewDetails")}
          </DropdownMenuItem>

          {product.isDeleted ? (
            <DropdownMenuItem onClick={() => setDialog("restore")}>
              {t("products.actions.restore")}
            </DropdownMenuItem>
          ) : (
            <>
              <DropdownMenuItem onClick={() => setDialog("edit")}>
                {t("products.actions.edit")}
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => setDialog("status")}>
                {t("products.actions.changeStatus")}
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => setDialog("variants")}>
                {t("products.actions.manageVariants")}
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => setDialog("images")}>
                {t("products.actions.manageImages")}
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                variant="destructive"
                onClick={() => setDialog("delete")}
              >
                {t("products.actions.delete")}
              </DropdownMenuItem>
            </>
          )}
        </DropdownMenuContent>
      </DropdownMenu>

      <ProductDetailsDialog
        product={product}
        open={dialog === "details"}
        onOpenChange={(open) => {
          if (!open) closeDialog();
        }}
      />
      <ProductDialog
        mode="edit"
        product={product}
        open={dialog === "edit"}
        onOpenChange={(open) => {
          if (!open) closeDialog();
        }}
      />
      <ProductStatusDialog
        product={product}
        open={dialog === "status"}
        onOpenChange={(open) => {
          if (!open) closeDialog();
        }}
      />
      <ProductVariantsDialog
        product={product}
        open={dialog === "variants"}
        onOpenChange={(open) => {
          if (!open) closeDialog();
        }}
      />
      <ProductImagesDialog
        product={product}
        open={dialog === "images"}
        onOpenChange={(open) => {
          if (!open) closeDialog();
        }}
      />
      <ProductDeleteDialog
        product={product}
        open={dialog === "delete"}
        onOpenChange={(open) => {
          if (!open) closeDialog();
        }}
      />
      <ProductRestoreDialog
        product={product}
        open={dialog === "restore"}
        onOpenChange={(open) => {
          if (!open) closeDialog();
        }}
      />
    </>
  );
}
