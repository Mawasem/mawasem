import i18n from "i18next";
import LanguageDetector from "i18next-browser-languagedetector";
import { initReactI18next } from "react-i18next";

import ar from "./ar.json";
import en from "./en.json";

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
        en: { translation: en },
        ar: { translation: ar },
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
