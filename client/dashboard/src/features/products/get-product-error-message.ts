import axios from "axios";
import type { TFunction } from "i18next";

interface ProblemDetailsResponse {
  detail?: string;
  code?: string;
}

export function getProductErrorMessage(error: unknown, t: TFunction) {
  if (axios.isAxiosError<ProblemDetailsResponse>(error)) {
    const code = error.response?.data?.code;

    switch (code) {
      case "products.not_found":
      case "product_variants.product_not_found":
        return t("products.errors.notFound");
      case "products.duplicate_slug":
        return t("products.errors.duplicateSlug");
      case "products.cannot_publish":
        return error.response?.data?.detail ?? t("products.errors.cannotPublish");
      case "products.invalid_reference":
        return error.response?.data?.detail ?? t("products.errors.invalidReference");
      case "product_variants.combination_already_exists":
        return t("products.errors.variantCombinationExists");
      case "product_variants.inconsistent_option_structure":
        return t("products.errors.inconsistentOptionStructure");
      case "product_variants.stock_concurrency_conflict":
        return t("products.errors.stockConcurrency");
      default:
        return error.response?.data?.detail ?? error.message ?? t("products.errors.unknown");
    }
  }

  return error instanceof Error ? error.message : t("products.errors.unknown");
}
