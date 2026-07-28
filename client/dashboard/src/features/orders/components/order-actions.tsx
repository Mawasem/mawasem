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

import { OrderStatus, type AdminOrderListItem, type OrderWorkflowAction } from "../types";
import { OrderDetailsDialog } from "./order-details-dialog";
import { OrderWorkflowDialog } from "./order-workflow-dialog";

export function OrderActions({ order }: { order: AdminOrderListItem }) {
  const { t } = useTranslation();
  const [detailsOpen, setDetailsOpen] = useState(false);
  const [workflowAction, setWorkflowAction] = useState<OrderWorkflowAction | null>(null);

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button variant="ghost" size="icon-sm" aria-label={t("orders.actions.openActions")}>
            <MoreHorizontal className="size-4" />
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
          <DropdownMenuItem onClick={() => setDetailsOpen(true)}>
            {t("orders.actions.viewDetails")}
          </DropdownMenuItem>

          {order.canConfirm || order.canReject || order.canCancel ||
          order.orderStatus === OrderStatus.Confirmed ||
          order.orderStatus === OrderStatus.Preparing ||
          order.orderStatus === OrderStatus.Shipped ? <DropdownMenuSeparator /> : null}

          {order.canConfirm ? (
            <DropdownMenuItem onClick={() => setWorkflowAction("confirm")}>{t("orders.actions.confirm")}</DropdownMenuItem>
          ) : null}
          {order.orderStatus === OrderStatus.Confirmed ? (
            <DropdownMenuItem onClick={() => setWorkflowAction("prepare")}>{t("orders.actions.prepare")}</DropdownMenuItem>
          ) : null}
          {order.orderStatus === OrderStatus.Preparing ? (
            <DropdownMenuItem onClick={() => setWorkflowAction("ship")}>{t("orders.actions.ship")}</DropdownMenuItem>
          ) : null}
          {order.orderStatus === OrderStatus.Shipped ? (
            <DropdownMenuItem onClick={() => setWorkflowAction("deliver")}>{t("orders.actions.deliver")}</DropdownMenuItem>
          ) : null}
          {order.canReject ? (
            <DropdownMenuItem variant="destructive" onClick={() => setWorkflowAction("reject")}>{t("orders.actions.reject")}</DropdownMenuItem>
          ) : null}
          {order.canCancel ? (
            <DropdownMenuItem variant="destructive" onClick={() => setWorkflowAction("cancel")}>{t("orders.actions.cancel")}</DropdownMenuItem>
          ) : null}
        </DropdownMenuContent>
      </DropdownMenu>

      <OrderDetailsDialog order={order} open={detailsOpen} onOpenChange={setDetailsOpen} />
      {workflowAction ? (
        <OrderWorkflowDialog
          order={order}
          action={workflowAction}
          open
          onOpenChange={(open) => {
            if (!open) setWorkflowAction(null);
          }}
        />
      ) : null}
    </>
  );
}
