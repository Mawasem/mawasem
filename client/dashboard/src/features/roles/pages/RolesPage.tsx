import { useState } from "react";
import { useTranslation } from "react-i18next";

import { EntityManagementPage } from "@/components/entity-management/EntityManagementPage";

import { useRoleColumns } from "../components/role-columns";
import { useRoles } from "../hooks/use-roles";

export default function RolesPage() {
  const { t } = useTranslation();
  const roleColumns = useRoleColumns();
  const [search] = useState("");
  const [includeDeleted] = useState(false);

  const { data, isLoading, error } = useRoles();

  const handleSearch = () => {
    return undefined;
  };

  const handleIncludeDeletedChange = () => {
    return undefined;
  };

  const handlePageChange = () => {
    return undefined;
  };

  return (
    <EntityManagementPage
      title={t("roles.page.title")}
      description={t("roles.page.description")}
      search={search}
      onSearch={handleSearch}
      includeDeleted={includeDeleted}
      onIncludeDeletedChange={handleIncludeDeletedChange}
      includeDeletedLabel={t("roles.filters.includeDeleted")}
      includeDeletedSwitchId="include-deleted-roles"
      buttonLabel={t("roles.actions.create")}
      onCreate={() => undefined}
      columns={roleColumns}
      data={data?.items ?? []}
      emptyStateLabel={t("roles.empty")}
      loading={isLoading}
      loadingLabel={t("roles.loading")}
      error={error}
      pagination={{
        totalCount: data?.items.length ?? 0,
        page: 1,
        totalPages: 1,
        totalCountLabel: t("roles.pagination.rows"),
        pageLabel: t("roles.pagination.page"),
        previousLabel: t("roles.pagination.previous"),
        nextLabel: t("roles.pagination.next"),
        onPageChange: handlePageChange,
      }}
      searchPlaceholder={t("roles.searchPlaceholder")}
    />
  );
}
