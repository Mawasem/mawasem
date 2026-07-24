import "@/lib/i18n";
import { useTranslation } from "react-i18next";
import { Check, ChevronsUpDown, Languages } from "lucide-react";

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar";

const languages = [
  {
    code: "en",
    labelKey: "sidebar.languageEnglish",
  },
  {
    code: "ar",
    labelKey: "sidebar.languageArabic",
  },
] as const;

export function LanguageSwitcher() {
  const { t, i18n } = useTranslation();

  const currentLanguage =
    i18n.resolvedLanguage === "ar"
      ? "ar"
      : "en";

  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <SidebarMenuButton
              size="lg"
              className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
            >
              <div className="flex aspect-square size-8 items-center justify-center rounded-lg bg-sidebar-primary text-sidebar-primary-foreground">
                <Languages className="size-4" />
              </div>

              <div className="grid flex-1 text-left text-sm leading-tight">
                <span className="truncate font-medium">
                  {t("sidebar.language")}
                </span>

                <span className="truncate text-xs">
                  {currentLanguage === "ar"
                    ? t("sidebar.languageArabic")
                    : t("sidebar.languageEnglish")}
                </span>
              </div>

              <ChevronsUpDown className="ml-auto size-4" />
            </SidebarMenuButton>
          </DropdownMenuTrigger>

          <DropdownMenuContent
            className="w-(--radix-dropdown-menu-trigger-width)"
            align="start"
          >
            {languages.map((language) => (
              <DropdownMenuItem
                key={language.code}
                onSelect={() => {
                  void i18n.changeLanguage(language.code);
                }}
              >
                {t(language.labelKey)}

                {currentLanguage === language.code ? (
                  <Check className="ms-auto size-4" />
                ) : null}
              </DropdownMenuItem>
            ))}
          </DropdownMenuContent>
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  );
}