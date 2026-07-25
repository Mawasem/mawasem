import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";
import { EntityDialog } from "@/components/entity-dialog/entity-dialog";

import { useCustomer } from "../hooks/use-customer";
import type { CustomerDetailsDialogProps } from "../types";



function formatDetailValue(
  value: string | number | boolean | null | undefined,
  kind: "default" | "currency" | "date" | "boolean" = "default",
  locale: string,
  t: (key: string) => string
) {
  if (
    value === null ||
    value === undefined ||
    value === ""
  ) {
    return t("customers.details.notAvailable");
  }

  if (kind === "boolean" && typeof value === "boolean") {
    return value
      ? t("customers.status.blocked")
      : t("customers.status.active");
  }

  if (kind === "currency" && typeof value === "number") {
    return new Intl.NumberFormat(locale, {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(value);
  }

  if (kind === "date" && typeof value === "string") {
    return value;
  }

  if (typeof value === "number") {
    return value.toString();
  }

  return String(value);
}

export function CustomerDetailsDialog({
  customer,
  open,
  onOpenChange,
}: CustomerDetailsDialogProps) {
  const { t, i18n } = useTranslation();
  const { customerData, isLoading, error } = useCustomer(
    customer.id,
    open && !!customer.id
  );

  const locale =
    i18n.resolvedLanguage === "ar"
      ? "ar-EG"
      : "en-US";

  const detailFields: Array<{
    label: string;
    value: string | number | boolean | null | undefined;
    kind?: "default" | "currency" | "date" | "boolean";
  }> = [
      {
        label: t("customers.details.fullNameAr"),
        value: customerData?.fullNameAr ?? null,
      },
      {
        label: t("customers.details.fullNameEn"),
        value: customerData?.fullNameEn ?? null,
      },
      {
        label: t("customers.details.phoneNumber"),
        value: customerData?.phoneNumber ?? null,
      },
      {
        label: t("customers.details.email"),
        value: customerData?.email ?? null,
      },
      {
        label: t("customers.details.birthDate"),
        value: customerData?.birthDate ?? null,
        kind: "date",
      },
      {
        label: t("customers.details.gender"),
        value: customerData?.gender ?? null,
      },
      {
        label: t("customers.details.referralSource"),
        value: customerData?.referralSource ?? null,
      },
      {
        label: t("customers.details.status"),
        value: customerData?.isBlocked ?? null,
        kind: "boolean",
      },
      {
        label: t("customers.details.blockedAt"),
        value: customerData?.blockedAt ?? null,
        kind: "date",
      },
      {
        label: t("customers.details.blockedReason"),
        value: customerData?.blockedReason ?? null,
      },
      {
        label: t("customers.details.totalOrders"),
        value: customerData?.totalOrders ?? null,
      },
      {
        label: t("customers.details.deliveredOrders"),
        value: customerData?.deliveredOrders ?? null,
      },
      {
        label: t("customers.details.totalSpent"),
        value: customerData?.totalSpent ?? null,
        kind: "currency",
      },
      {
        label: t("customers.details.savedAddressCount"),
        value: customerData?.savedAddressCount ?? null,
      },
      {
        label: t("customers.details.reviewCount"),
        value: customerData?.reviewCount ?? null,
      },
    ];

  const renderBody = () => {
    if (isLoading) {
      return (
        <p className="text-sm text-muted-foreground">
          {t("customers.details.loading")}
        </p>
      );
    }

    if (error instanceof Error) {
      return (
        <p className="text-sm text-destructive">
          {t("customers.errors.generic", {
            message: error.message,
          })}
        </p>
      );
    }

    if (!customerData) {
      return null;
    }

    return (
      <div className="space-y-4">
        <div className="rounded-md border p-4">
          <p className="text-sm font-medium">
            {i18n.resolvedLanguage === "ar"
              ? customerData.fullNameAr
              : customerData.fullNameEn}
          </p>
        </div>

        <div className="grid gap-4 md:grid-cols-2">
          {detailFields.map((detail) => (
            <div key={detail.label} className="space-y-1">
              <p className="text-sm text-muted-foreground">
                {detail.label}
              </p>
              {detail.label === t("customers.details.status") ? (
                <Badge
                  variant={
                    customerData.isBlocked
                      ? "secondary"
                      : "default"
                  }
                >
                  {formatDetailValue(
                    detail.value,
                    detail.kind,
                    locale,
                    t
                  )}
                </Badge>
              ) : (
                <p className="text-sm font-medium">
                  {formatDetailValue(
                    detail.value,
                    detail.kind,
                    locale,
                    t
                  )}
                </p>
              )}
            </div>
          ))}
        </div>
      </div>
    );
  };

  return (
    <EntityDialog
      open={open}
      onOpenChange={onOpenChange}
      title={t("customers.details.title")}
      description={t("customers.details.description")}
    >
      {renderBody()}
    </EntityDialog>
  );
}
