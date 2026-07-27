import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";

import { EntityDialog } from "@/components/entity-dialog/entity-dialog";
import { Badge } from "@/components/ui/badge";

import { getDeliveryAreaErrorMessage } from "../get-delivery-area-error-message";
import { useDeliveryArea } from "../hooks/use-delivery-area";
import type { DeliveryAreaDetailsDialogProps } from "../types";
import { DeliveryAreaStatusBadge } from "./delivery-area-status-badge";

interface DetailItemProps {
  label: string;
  value: ReactNode;
}

function DetailItem({ label, value }: DetailItemProps) {
  return (
    <div className="space-y-1">
      <p className="text-sm text-muted-foreground">{label}</p>
      <div className="text-sm font-medium">{value}</div>
    </div>
  );
}

export function DeliveryAreaDetailsDialog({
  deliveryArea,
  open,
  onOpenChange,
}: DeliveryAreaDetailsDialogProps) {
  const { t, i18n } = useTranslation();

  const { deliveryAreaData, isLoading, error } = useDeliveryArea(
    deliveryArea.id,
    open
  );

  const locale =
    i18n.resolvedLanguage === "ar" ? "ar-EG" : "en-GB";

  const formatAmount = (value: number) =>
    new Intl.NumberFormat(locale, {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(value);

  const formatDate = (value: string | null) => {
    if (!value) {
      return t("common.notAvailable");
    }

    return new Intl.DateTimeFormat(locale, {
      dateStyle: "medium",
      timeStyle: "short",
    }).format(new Date(value));
  };

  const renderContent = () => {
    if (isLoading) {
      return (
        <p className="text-sm text-muted-foreground">
          {t("deliveryAreas.details.loading")}
        </p>
      );
    }

    if (error) {
      return (
        <p className="text-sm text-destructive">
          {getDeliveryAreaErrorMessage(error, t)}
        </p>
      );
    }

    if (!deliveryAreaData) {
      return null;
    }

    const displayName =
      i18n.resolvedLanguage === "ar"
        ? deliveryAreaData.nameAr
        : deliveryAreaData.nameEn;

    return (
      <div className="max-h-[70vh] space-y-6 overflow-y-auto pe-1">
        <div className="flex flex-wrap items-center justify-between gap-3 rounded-2xl border p-4">
          <div>
            <p className="font-semibold">{displayName}</p>
            <p className="text-sm text-muted-foreground">
              {deliveryAreaData.nameAr} · {deliveryAreaData.nameEn}
            </p>
          </div>

          <div className="flex flex-wrap gap-2">
            <DeliveryAreaStatusBadge status={deliveryAreaData.status} />

            <Badge
              variant={deliveryAreaData.isActive ? "default" : "secondary"}
            >
              {deliveryAreaData.isActive
                ? t("deliveryAreas.activity.active")
                : t("deliveryAreas.activity.inactive")}
            </Badge>

            {deliveryAreaData.isDeleted ? (
              <Badge variant="destructive">
                {t("deliveryAreas.activity.deleted")}
              </Badge>
            ) : null}
          </div>
        </div>

        <div className="grid gap-4 md:grid-cols-2">
          <DetailItem
            label={t("deliveryAreas.details.nameAr")}
            value={deliveryAreaData.nameAr}
          />
          <DetailItem
            label={t("deliveryAreas.details.nameEn")}
            value={deliveryAreaData.nameEn}
          />
          <DetailItem
            label={t("deliveryAreas.details.deliveryFee")}
            value={formatAmount(deliveryAreaData.deliveryFee)}
          />
          <DetailItem
            label={t("deliveryAreas.details.effectiveDeliveryFee")}
            value={formatAmount(deliveryAreaData.effectiveDeliveryFee)}
          />
          <DetailItem
            label={t("deliveryAreas.details.freeDelivery")}
            value={
              <Badge
                variant={
                  deliveryAreaData.isFreeDelivery ? "default" : "outline"
                }
              >
                {deliveryAreaData.isFreeDelivery
                  ? t("common.yes")
                  : t("common.no")}
              </Badge>
            }
          />
          <DetailItem
            label={t("deliveryAreas.details.activeAddresses")}
            value={deliveryAreaData.activeAddressCount}
          />
        </div>

        <div className="border-t pt-5">
          <h3 className="mb-4 font-semibold">
            {t("deliveryAreas.details.auditTitle")}
          </h3>

          <div className="grid gap-4 md:grid-cols-2">
            <DetailItem
              label={t("deliveryAreas.details.createdOn")}
              value={formatDate(deliveryAreaData.createdOn)}
            />
            <DetailItem
              label={t("deliveryAreas.details.createdBy")}
              value={
                deliveryAreaData.createdBy ?? t("common.notAvailable")
              }
            />
            <DetailItem
              label={t("deliveryAreas.details.lastModifiedOn")}
              value={formatDate(deliveryAreaData.lastModifiedOn)}
            />
            <DetailItem
              label={t("deliveryAreas.details.lastModifiedBy")}
              value={
                deliveryAreaData.lastModifiedBy ??
                t("common.notAvailable")
              }
            />

            {deliveryAreaData.isDeleted ? (
              <>
                <DetailItem
                  label={t("deliveryAreas.details.deletedOn")}
                  value={formatDate(deliveryAreaData.deletedOn)}
                />
                <DetailItem
                  label={t("deliveryAreas.details.deletedBy")}
                  value={
                    deliveryAreaData.deletedBy ??
                    t("common.notAvailable")
                  }
                />
              </>
            ) : null}
          </div>
        </div>
      </div>
    );
  };

  return (
    <EntityDialog
      open={open}
      onOpenChange={onOpenChange}
      title={t("deliveryAreas.details.title")}
      description={t("deliveryAreas.details.description")}
    >
      {renderContent()}
    </EntityDialog>
  );
}
