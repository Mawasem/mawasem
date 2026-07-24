import * as React from "react";
import "@/lib/i18n";
import { useTranslation } from "react-i18next";

import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarRail,
} from "@/components/ui/sidebar";

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";
import {
  ChevronRight,
} from "lucide-react";


import { data } from "@/lib/data";
import { NavLink } from "react-router-dom";
import { NavUser } from "./nav-user";
import { LanguageSwitcher } from "./language-switcher";


export function AppSidebar(
  props: React.ComponentProps<typeof Sidebar>
) {
  const { t, i18n } = useTranslation();

  const sidebarDirection =
    i18n.dir(i18n.resolvedLanguage) === "rtl"
      ? "rtl"
      : "ltr";

  const sidebarSide =
    sidebarDirection === "rtl"
      ? "right"
      : "left";

  return (
    <Sidebar
      {...props}
      dir={sidebarDirection}
      side={sidebarSide}
    >
      <SidebarContent>
        {data.map((group) => (
          <Collapsible
            key={group.key}
            defaultOpen
            className="group/collapsible"
          >
            <SidebarGroup>
              <SidebarGroupLabel asChild>
                <CollapsibleTrigger className="flex w-full items-center justify-between">
                  <span>{t(`sidebar.${group.key}`)}</span>

                  <ChevronRight className="size-4 transition-transform group-data-[state=open]/collapsible:rotate-90" />
                </CollapsibleTrigger>
              </SidebarGroupLabel>

              <CollapsibleContent>
                <SidebarGroupContent>
                  <SidebarMenu>
                    {group.items.map((item) => (
                      <SidebarMenuItem key={item.key}>
                        <SidebarMenuButton asChild>
                          <NavLink to={item.url}>
                            <item.icon />
                            <span>{t(`sidebar.${item.key}`)}</span>
                          </NavLink>
                        </SidebarMenuButton>
                      </SidebarMenuItem>
                    ))}
                  </SidebarMenu>
                </SidebarGroupContent>
              </CollapsibleContent>
            </SidebarGroup>
          </Collapsible>
        ))}
      </SidebarContent>

      <SidebarFooter>
        <LanguageSwitcher />
        <NavUser />
      </SidebarFooter>

      <SidebarRail />
    </Sidebar>
  );
}