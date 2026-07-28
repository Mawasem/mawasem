import { Badge } from "@/components/ui/badge";
import { useTranslation } from "react-i18next";
import { getOrderStatusKey } from "../order-utils";
import { OrderStatus } from "../types";

export function OrderStatusBadge({ status }: { status: OrderStatus }) {
  const { t } = useTranslation();
  const variant =
    status === OrderStatus.Delivered || status === OrderStatus.Confirmed
      ? "default"
      : status === OrderStatus.Cancelled || status === OrderStatus.Rejected
        ? "destructive"
        : status === OrderStatus.Pending || status === OrderStatus.Preparing
          ? "secondary"
          : "outline";

  return <Badge variant={variant}>{t(`orders.status.${getOrderStatusKey(status)}`)}</Badge>;
}
