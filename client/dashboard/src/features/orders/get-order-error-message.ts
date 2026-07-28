import axios from "axios";
import type { TFunction } from "i18next";

interface ProblemDetails {
  detail?: string;
  title?: string;
  code?: string;
}

export function getOrderErrorMessage(error: unknown, t: TFunction) {
  if (axios.isAxiosError<ProblemDetails>(error)) {
    const code = error.response?.data?.code;
    const codeKey = code?.replaceAll(".", "_");

    if (codeKey && t(`orders.errors.codes.${codeKey}`, { defaultValue: "" })) {
      return t(`orders.errors.codes.${codeKey}`);
    }

    return (
      error.response?.data?.detail ??
      error.response?.data?.title ??
      t("orders.errors.generic")
    );
  }

  return error instanceof Error ? error.message : t("orders.errors.generic");
}
