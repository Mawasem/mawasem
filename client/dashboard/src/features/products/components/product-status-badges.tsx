import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";

interface ProductStatusBadgesProps {
  isPublished: boolean;
  isFeatured: boolean;
  isDeleted?: boolean;
}

export function ProductStatusBadges({
  isPublished,
  isFeatured,
  isDeleted = false,
}: ProductStatusBadgesProps) {
  const { t } = useTranslation();

  if (isDeleted) {
    return (
      <Badge variant="destructive">{t("products.status.deleted")}</Badge>
    );
  }

  return (
    <div className="flex flex-wrap gap-2">
      <Badge variant={isPublished ? "default" : "secondary"}>
        {isPublished
          ? t("products.status.published")
          : t("products.status.draft")}
      </Badge>

      {isFeatured ? (
        <Badge variant="outline">{t("products.status.featured")}</Badge>
      ) : null}
    </div>
  );
}
