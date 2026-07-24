import i18n from "i18next";
import LanguageDetector from "i18next-browser-languagedetector";
import { initReactI18next } from "react-i18next";

import arAuth from "./ar/auth.json";
import arBrands from "./ar/brands.json";
import arCategories from "./ar/categories.json";
import arCollections from "./ar/collections.json";
import arCommon from "./ar/common.json";
import arCustomers from "./ar/customers.json";
import arDashboard from "./ar/dashboard.json";
import arEmployees from "./ar/employees.json";
import arOrders from "./ar/orders.json";
import arProducts from "./ar/products.json";
import arRoles from "./ar/roles.json";
import arSeasons from "./ar/seasons.json";
import arSettings from "./ar/settings.json";
import arSidebar from "./ar/sidebar.json";
import enAuth from "./en/auth.json";
import enBrands from "./en/brands.json";
import enCategories from "./en/categories.json";
import enCollections from "./en/collections.json";
import enCommon from "./en/common.json";
import enCustomers from "./en/customers.json";
import enDashboard from "./en/dashboard.json";
import enEmployees from "./en/employees.json";
import enOrders from "./en/orders.json";
import enProducts from "./en/products.json";
import enRoles from "./en/roles.json";
import enSeasons from "./en/seasons.json";
import enSettings from "./en/settings.json";
import enSidebar from "./en/sidebar.json";

const getDirection = (language: string) =>
  language === "ar"
    ? "rtl"
    : "ltr";

const syncDocumentLanguage = (language: string) => {
  if (typeof document === "undefined") {
    return;
  }

  document.documentElement.lang = language;
  document.documentElement.dir = getDirection(language);
};

if (!i18n.isInitialized) {
  void i18n
    .use(LanguageDetector)
    .use(initReactI18next)
    .init({
      resources: {
        en: {
          translation: {
            ...enCommon,
            ...enSidebar,
            ...enDashboard,
            ...enCategories,
            ...enBrands,
            ...enCollections,
            ...enSeasons,
            ...enCustomers,
            ...enProducts,
            ...enOrders,
            ...enRoles,
            ...enEmployees,
            ...enSettings,
            ...enAuth,
          },
        },
        ar: {
          translation: {
            ...arCommon,
            ...arSidebar,
            ...arDashboard,
            ...arCategories,
            ...arBrands,
            ...arCollections,
            ...arSeasons,
            ...arCustomers,
            ...arProducts,
            ...arOrders,
            ...arRoles,
            ...arEmployees,
            ...arSettings,
            ...arAuth,
          },
        },
      },
      supportedLngs: ["en", "ar"],
      detection: {
        order: ["localStorage", "navigator"],
        caches: ["localStorage"],
      },
      fallbackLng: "en",
      interpolation: {
        escapeValue: false,
      },
    });
}

syncDocumentLanguage(i18n.resolvedLanguage ?? "en");
i18n.on("languageChanged", syncDocumentLanguage);

export default i18n;
