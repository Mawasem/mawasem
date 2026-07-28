import { useState } from "react";
import { useTranslation } from "react-i18next";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";

import { getOrderErrorMessage } from "../get-order-error-message";
import { useCancelOrder } from "../hooks/use-cancel-order";
import { useConfirmOrder } from "../hooks/use-confirm-order";
import { useDeliverOrder } from "../hooks/use-deliver-order";
import { usePrepareOrder } from "../hooks/use-prepare-order";
import { useRejectOrder } from "../hooks/use-reject-order";
import { useShipOrder } from "../hooks/use-ship-order";
import type { AdminOrderListItem, OrderWorkflowAction } from "../types";

interface Props {
  order: Pick<AdminOrderListItem, "id" | "orderNumber">;
  action: OrderWorkflowAction;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function OrderWorkflowDialog({ order, action, open, onOpenChange }: Props) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">
        {open ? (
          <OrderWorkflowContent
            key={`${order.id}-${action}`}
            order={order}
            action={action}
            onClose={() => onOpenChange(false)}
          />
        ) : null}
      </DialogContent>
    </Dialog>
  );
}

function OrderWorkflowContent({
  order,
  action,
  onClose,
}: {
  order: Pick<AdminOrderListItem, "id" | "orderNumber">;
  action: OrderWorkflowAction;
  onClose: () => void;
}) {
  const { t } = useTranslation();
  const [reason, setReason] = useState("");
  const confirmMutation = useConfirmOrder();
  const prepareMutation = usePrepareOrder();
  const shipMutation = useShipOrder();
  const deliverMutation = useDeliverOrder();
  const rejectMutation = useRejectOrder();
  const cancelMutation = useCancelOrder();

  const mutation = {
    confirm: confirmMutation,
    prepare: prepareMutation,
    ship: shipMutation,
    deliver: deliverMutation,
    reject: rejectMutation,
    cancel: cancelMutation,
  }[action];

  const requiresReason = action === "reject" || action === "cancel";
  const trimmedReason = reason.trim();
  const isLoading = mutation.isLoading;

  const handleSubmit = async () => {
    try {
      if (action === "confirm") await confirmMutation.confirmOrderAsync({ orderId: order.id });
      if (action === "prepare") await prepareMutation.prepareOrderAsync({ orderId: order.id });
      if (action === "ship") await shipMutation.shipOrderAsync({ orderId: order.id });
      if (action === "deliver") await deliverMutation.deliverOrderAsync({ orderId: order.id });
      if (action === "reject") {
        await rejectMutation.rejectOrderAsync({ orderId: order.id, data: { reason: trimmedReason } });
      }
      if (action === "cancel") {
        await cancelMutation.cancelOrderAsync({ orderId: order.id, data: { reason: trimmedReason } });
      }
      onClose();
    } catch {
      // The mutation error is rendered below.
    }
  };

  return (
    <>
      <DialogHeader>
        <DialogTitle>{t(`orders.workflow.${action}.title`)}</DialogTitle>
        <DialogDescription>
          {t(`orders.workflow.${action}.description`, { orderNumber: order.orderNumber })}
        </DialogDescription>
      </DialogHeader>

      {requiresReason ? (
        <div className="space-y-2 py-2">
          <Label htmlFor={`order-${action}-reason`}>{t("orders.workflow.reason")}</Label>
          <Textarea
            id={`order-${action}-reason`}
            value={reason}
            onChange={(event) => setReason(event.target.value)}
            placeholder={t("orders.workflow.reasonPlaceholder")}
            maxLength={1000}
            disabled={isLoading}
          />
        </div>
      ) : null}

      {mutation.error ? (
        <p className="text-sm text-destructive">
          {getOrderErrorMessage(mutation.error, t)}
        </p>
      ) : null}

      <DialogFooter>
        <Button type="button" variant="outline" onClick={onClose} disabled={isLoading}>
          {t("common.cancel")}
        </Button>
        <Button
          type="button"
          variant={action === "reject" || action === "cancel" ? "destructive" : "default"}
          onClick={handleSubmit}
          disabled={isLoading || (requiresReason && !trimmedReason)}
        >
          {isLoading ? t("common.saving") : t(`orders.workflow.${action}.submit`)}
        </Button>
      </DialogFooter>
    </>
  );
}
