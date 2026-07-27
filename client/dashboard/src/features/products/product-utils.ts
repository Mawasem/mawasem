import { api } from "@/lib/axios";

export function resolveProductImageUrl(imageUrl: string) {
  if (/^https?:\/\//i.test(imageUrl)) {
    return imageUrl;
  }

  const baseUrl = api.defaults.baseURL;

  if (!baseUrl) {
    return imageUrl;
  }

  try {
    const apiUrl = new URL(baseUrl, window.location.origin);
    return new URL(imageUrl, apiUrl.origin).toString();
  } catch {
    return imageUrl;
  }
}

export function formatProductPrice(value: number, language: string) {
  return new Intl.NumberFormat(language === "ar" ? "ar-EG" : "en-GB", {
    style: "currency",
    currency: "EGP",
    maximumFractionDigits: 2,
  }).format(value);
}
