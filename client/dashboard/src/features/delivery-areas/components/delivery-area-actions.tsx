import { MoreHorizontal } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import type { DeliveryAreaActionsProps } from "../types";
import { DeliveryAreaDeleteDialog } from "./delivery-area-delete-dialog";
import { DeliveryAreaDetailsDialog } from "./delivery-area-details-dialog";
import { DeliveryAreaDialog } from "./delivery-area-dialog";
import { DeliveryAreaRestoreDialog } from "./delivery-area-restore-dialog";
import { DeliveryAreaStatusDialog } from "./delivery-area-status-dialog";

export function DeliveryAreaActions({
  deliveryArea,
}: DeliveryAreaActionsProps) {
  const { t } = useTranslation();

  const [isDetailsDialogOpen, setIsDetailsDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const [isStatusDialogOpen, setIsStatusDialogOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isRestoreDialogOpen, setIsRestoreDialogOpen] = useState(false);

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            size="icon-sm"
            aria-label={t("deliveryAreas.actions.openActions")}
          >
            <MoreHorizontal className="size-4" />
          </Button>
        </DropdownMenuTrigger>

        <DropdownMenuContent align="end">
          <DropdownMenuItem onClick={() => setIsDetailsDialogOpen(true)}>
            {t("deliveryAreas.actions.viewDetails")}
          </DropdownMenuItem>

          {deliveryArea.isDeleted ? (
            <DropdownMenuItem onClick={() => setIsRestoreDialogOpen(true)}>
              {t("deliveryAreas.actions.restore")}
            </DropdownMenuItem>
          ) : (
            <>
              <DropdownMenuItem onClick={() => setIsEditDialogOpen(true)}>
                {t("deliveryAreas.actions.edit")}
              </DropdownMenuItem>

              <DropdownMenuItem onClick={() => setIsStatusDialogOpen(true)}>
                {t("deliveryAreas.actions.changeStatus")}
              </DropdownMenuItem>

              <DropdownMenuItem
                variant="destructive"
                onClick={() => setIsDeleteDialogOpen(true)}
              >
                {t("deliveryAreas.actions.delete")}
              </DropdownMenuItem>
            </>
          )}
        </DropdownMenuContent>
      </DropdownMenu>

      <DeliveryAreaDetailsDialog
        deliveryArea={deliveryArea}
        open={isDetailsDialogOpen}
        onOpenChange={setIsDetailsDialogOpen}
      />

      <DeliveryAreaDialog
        mode="edit"
        deliveryArea={deliveryArea}
        open={isEditDialogOpen}
        onOpenChange={setIsEditDialogOpen}
      />

      <DeliveryAreaStatusDialog
        deliveryArea={deliveryArea}
        open={isStatusDialogOpen}
        onOpenChange={setIsStatusDialogOpen}
      />

      <DeliveryAreaDeleteDialog
        deliveryArea={deliveryArea}
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
      />

      <DeliveryAreaRestoreDialog
        deliveryArea={deliveryArea}
        open={isRestoreDialogOpen}
        onOpenChange={setIsRestoreDialogOpen}
      />
    </>
  );
}
