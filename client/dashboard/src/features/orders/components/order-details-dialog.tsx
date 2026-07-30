import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";

import { getOrderErrorMessage } from "../get-order-error-message";
import { useAdminOrder } from "../hooks/use-admin-order";
import {
  formatOrderDate,
  formatOrderMoney,
  getDeliveryMethodKey,
  getOrderSourceKey,
  getPaymentMethodKey,
  getPaymentStatusKey,
} from "../order-utils";
import { DeliveryMethod, type AdminOrderListItem } from "../types";
import { OrderStatusBadge } from "./order-status-badge";

interface Props {
  order: Pick<AdminOrderListItem, "id" | "orderNumber">;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function OrderDetailsDialog({ order, open, onOpenChange }: Props) {
  const { t, i18n } = useTranslation();
  const { orderData, isLoading, error } = useAdminOrder(order.id, open);
  const language = i18n.resolvedLanguage ?? "en";
  const isStorePickup =
    orderData?.deliveryMethod === DeliveryMethod.StorePickup;

  const localized = (ar?: string | null, en?: string | null) =>
    language === "ar" ? ar || en || "-" : en || ar || "-";

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] overflow-y-auto sm:max-w-5xl">
        <DialogHeader>
          <DialogTitle>{t("orders.details.title", { orderNumber: order.orderNumber })}</DialogTitle>
          <DialogDescription>{t("orders.details.description")}</DialogDescription>
        </DialogHeader>

        {isLoading ? (
          <div className="space-y-4">
            <Skeleton className="h-24 w-full" />
            <Skeleton className="h-48 w-full" />
            <Skeleton className="h-48 w-full" />
          </div>
        ) : null}

        {error ? (
          <p className="text-sm text-destructive">{getOrderErrorMessage(error, t)}</p>
        ) : null}

        {orderData ? (
          <div className="space-y-6">
            <div className="flex flex-wrap items-center justify-between gap-3 rounded-lg border p-4">
              <div>
                <p className="font-mono text-lg font-semibold">{orderData.orderNumber}</p>
                <p className="text-sm text-muted-foreground">
                  {formatOrderDate(orderData.orderDate, language)}
                </p>
              </div>
              <div className="flex flex-wrap gap-2">
                <OrderStatusBadge status={orderData.orderStatus} />
                <Badge variant="outline">
                  {t(`orders.paymentStatus.${getPaymentStatusKey(orderData.paymentStatus)}`)}
                </Badge>
              </div>
            </div>

            <div className="grid gap-4 lg:grid-cols-2">
              <Card>
                <CardHeader><CardTitle>{t("orders.details.customer")}</CardTitle></CardHeader>
                <CardContent className="space-y-2 text-sm">
                  <DetailRow label={t("orders.fields.name")} value={localized(orderData.customer.nameAr, orderData.customer.nameEn)} />
                  <DetailRow label={t("orders.fields.phone")} value={orderData.customer.phone} />
                  <DetailRow label={t("orders.fields.customerId")} value={String(orderData.customer.userId)} />
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="flex-row items-center justify-between">
                  <CardTitle>{t("orders.details.shipping")}</CardTitle>
                  {isStorePickup ? (
                    <Badge variant="secondary">
                      {t("orders.deliveryMethod.store_pickup")}
                    </Badge>
                  ) : null}
                </CardHeader>
                <CardContent className="space-y-2 text-sm">
                  <DetailRow label={t("orders.fields.deliveryMethod")} value={t(`orders.deliveryMethod.${getDeliveryMethodKey(orderData.deliveryMethod)}`)} />
                  {isStorePickup ? (
                    <p className="rounded-md bg-muted p-3 text-muted-foreground">
                      {t("orders.details.noDeliveryAddress")}
                    </p>
                  ) : (
                    <>
                      <DetailRow label={t("orders.fields.deliveryArea")} value={localized(orderData.shipping.deliveryAreaNameAr, orderData.shipping.deliveryAreaNameEn)} />
                      <DetailRow label={t("orders.fields.recipient")} value={orderData.shipping.recipientName ?? "-"} />
                      <DetailRow label={t("orders.fields.phone")} value={orderData.shipping.recipientPhone ?? "-"} />
                      <DetailRow label={t("orders.fields.address")} value={[
                        orderData.shipping.city,
                        orderData.shipping.areaName,
                        orderData.shipping.detailedAddress,
                        orderData.shipping.buildingNumber && `${t("orders.fields.building")}: ${orderData.shipping.buildingNumber}`,
                        orderData.shipping.floorNumber && `${t("orders.fields.floor")}: ${orderData.shipping.floorNumber}`,
                        orderData.shipping.apartmentNumber && `${t("orders.fields.apartment")}: ${orderData.shipping.apartmentNumber}`,
                      ].filter(Boolean).join(" - ") || "-"} />
                      <DetailRow label={t("orders.fields.landmark")} value={orderData.shipping.landmark ?? "-"} />
                    </>
                  )}
                </CardContent>
              </Card>
            </div>

            <Card>
              <CardHeader><CardTitle>{t("orders.details.orderInfo")}</CardTitle></CardHeader>
              <CardContent className="grid gap-3 text-sm sm:grid-cols-2 lg:grid-cols-4">
                <DetailRow label={t("orders.fields.paymentMethod")} value={t(`orders.paymentMethod.${getPaymentMethodKey(orderData.paymentMethod)}`)} />
                <DetailRow label={t("orders.fields.paymentStatus")} value={t(`orders.paymentStatus.${getPaymentStatusKey(orderData.paymentStatus)}`)} />
                <DetailRow label={t("orders.fields.source")} value={t(`orders.source.${getOrderSourceKey(orderData.orderSource)}`)} />
                <DetailRow label={t("orders.fields.coupon")} value={orderData.couponCode ?? "-"} />
                <DetailRow label={t("orders.fields.notes")} value={orderData.notes ?? "-"} />
                <DetailRow label={t("orders.fields.cancellationReason")} value={orderData.cancellationReason ?? "-"} />
                <DetailRow label={t("orders.fields.rejectionReason")} value={orderData.rejectionReason ?? "-"} />
                <DetailRow label={t("orders.fields.stockRestoredAt")} value={orderData.stockRestoredAtUtc ? formatOrderDate(orderData.stockRestoredAtUtc, language) : "-"} />
              </CardContent>
            </Card>

            <Card>
              <CardHeader><CardTitle>{t("orders.details.items")}</CardTitle></CardHeader>
              <CardContent className="space-y-4">
                {orderData.items.map((item) => (
                  <div key={item.id} className="rounded-lg border p-4">
                    <div className="flex flex-wrap justify-between gap-3">
                      <div>
                        <p className="font-medium">{localized(item.productNameAr, item.productNameEn)}</p>
                        <p className="text-sm text-muted-foreground">
                          {localized(item.variantSummaryAr, item.variantSummaryEn)} · {item.sku}
                        </p>
                      </div>
                      <p className="font-semibold">{formatOrderMoney(item.lineTotal, language)}</p>
                    </div>
                    <Separator className="my-3" />
                    <div className="grid gap-2 text-sm sm:grid-cols-4">
                      <DetailRow label={t("orders.fields.quantity")} value={String(item.quantity)} />
                      <DetailRow label={t("orders.fields.unitPrice")} value={formatOrderMoney(item.unitPrice, language)} />
                      <DetailRow label={t("orders.fields.discount")} value={formatOrderMoney(item.discountAmount, language)} />
                      <DetailRow label={t("orders.fields.refundedQuantity")} value={String(item.refundedQuantity)} />
                    </div>
                  </div>
                ))}
              </CardContent>
            </Card>

            <Card>
              <CardHeader><CardTitle>{t("orders.details.totals")}</CardTitle></CardHeader>
              <CardContent className="ms-auto w-full space-y-2 text-sm sm:max-w-md">
                <DetailRow label={t("orders.fields.subTotal")} value={formatOrderMoney(orderData.subTotal, language)} />
                <DetailRow label={t("orders.fields.discount")} value={formatOrderMoney(orderData.discount, language)} />
                <DetailRow label={t("orders.fields.deliveryFee")} value={formatOrderMoney(orderData.deliveryFee, language)} />
                <Separator />
                <DetailRow label={t("orders.fields.total")} value={formatOrderMoney(orderData.totalAmount, language)} strong />
              </CardContent>
            </Card>
          </div>
        ) : null}
      </DialogContent>
    </Dialog>
  );
}

function DetailRow({ label, value, strong = false }: { label: string; value: string; strong?: boolean }) {
  return (
    <div className="flex min-w-0 items-start justify-between gap-3">
      <span className="text-muted-foreground">{label}</span>
      <span className={`break-words text-end ${strong ? "text-base font-bold" : "font-medium"}`}>{value}</span>
    </div>
  );
}
