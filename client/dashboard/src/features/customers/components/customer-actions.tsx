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

import type { CustomerActionsProps } from "../types";
import { BlockCustomerDialog } from "./block-customer-dialog";
import { UnblockCustomerDialog } from "./unblock-customer-dialog";

export function CustomerActions({
  customer,
}: CustomerActionsProps) {
  const { t } = useTranslation();

  const [isBlockDialogOpen, setIsBlockDialogOpen] =
    useState(false);
  const [isUnblockDialogOpen, setIsUnblockDialogOpen] =
    useState(false);

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            size="icon-sm"
            aria-label={t("customers.actions.openActions")}
          >
            <MoreHorizontal className="size-4" />
          </Button>
        </DropdownMenuTrigger>

        <DropdownMenuContent align="end">
          {customer.isBlocked ? (
            <DropdownMenuItem
              onClick={() =>
                setIsUnblockDialogOpen(true)
              }
            >
              {t("customers.actions.unblock")}
            </DropdownMenuItem>
          ) : (
            <DropdownMenuItem
              variant="destructive"
              onClick={() =>
                setIsBlockDialogOpen(true)
              }
            >
              {t("customers.actions.block")}
            </DropdownMenuItem>
          )}
        </DropdownMenuContent>
      </DropdownMenu>

      <BlockCustomerDialog
        customer={customer}
        open={isBlockDialogOpen}
        onOpenChange={setIsBlockDialogOpen}
      />

      <UnblockCustomerDialog
        customer={customer}
        open={isUnblockDialogOpen}
        onOpenChange={setIsUnblockDialogOpen}
      />
    </>
  );
}
