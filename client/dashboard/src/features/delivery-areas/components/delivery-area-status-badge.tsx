import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";

import { DeliveryAreaStatus } from "../types";

interface DeliveryAreaStatusBadgeProps {
  status: DeliveryAreaStatus;
}

export function DeliveryAreaStatusBadge({
  status,
}: DeliveryAreaStatusBadgeProps) {
  const { t } = useTranslation();

  switch (status) {
    case DeliveryAreaStatus.Pending:
      return (
        <Badge variant="secondary">
          {t("deliveryAreas.status.pending")}
        </Badge>
      );

    case DeliveryAreaStatus.Confirmed:
      return (
        <Badge variant="default">
          {t("deliveryAreas.status.confirmed")}
        </Badge>
      );

    case DeliveryAreaStatus.Restricted:
      return (
        <Badge variant="destructive">
          {t("deliveryAreas.status.restricted")}
        </Badge>
      );

    default:
      return <Badge variant="outline">{String(status)}</Badge>;
  }
}
