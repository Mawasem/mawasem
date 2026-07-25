import { useTranslation } from "react-i18next";

import { EntityPagination } from "@/components/entity-table/entity-pagination";
import { EntityTable } from "@/components/entity-table/entity-table";

import { useRoleColumns } from "../components/role-columns";
import { useRoles } from "../hooks/use-roles";

export default function RolesPage() {
  const { t } = useTranslation();
  const roleColumns = useRoleColumns();

  const { data, isLoading, error } = useRoles();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">{t("roles.page.title")}</h1>
        <p className="text-muted-foreground">{t("roles.page.description")}</p>
      </div>

      <EntityTable
        columns={roleColumns}
        data={data?.items ?? []}
        emptyStateLabel={t("roles.empty")}
      />

      {isLoading ? (
        <p className="text-sm text-muted-foreground">{t("roles.loading")}</p>
      ) : null}

      {error instanceof Error ? (
        <p className="text-sm text-destructive">{error.message}</p>
      ) : null}

      <EntityPagination
        totalCount={data?.items.length ?? 0}
        page={1}
        totalPages={1}
        totalCountLabel={t("roles.pagination.rows")}
        pageLabel={t("roles.pagination.page")}
        previousLabel={t("roles.pagination.previous")}
        nextLabel={t("roles.pagination.next")}
        onPageChange={() => undefined}
      />
    </div>
  );
}
