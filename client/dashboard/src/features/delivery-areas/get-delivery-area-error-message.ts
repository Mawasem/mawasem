import axios from "axios";
import type { TFunction } from "i18next";

interface ProblemDetailsResponse {
  detail?: string;
  code?: string;
}

export function getDeliveryAreaErrorMessage(
  error: unknown,
  t: TFunction
) {
  if (axios.isAxiosError<ProblemDetailsResponse>(error)) {
    const code = error.response?.data?.code;

    switch (code) {
      case "delivery_areas.duplicate_name":
        return t("deliveryAreas.errors.duplicateName");

      case "delivery_areas.has_active_addresses":
        return t("deliveryAreas.errors.hasActiveAddresses");

      case "delivery_areas.not_found":
        return t("deliveryAreas.errors.notFound");

      case "delivery_areas.invalid_request":
        return (
          error.response?.data?.detail ??
          t("deliveryAreas.errors.invalidRequest")
        );

      default:
        return (
          error.response?.data?.detail ??
          error.message ??
          t("deliveryAreas.errors.unknown")
        );
    }
  }

  return error instanceof Error
    ? error.message
    : t("deliveryAreas.errors.unknown");
}
