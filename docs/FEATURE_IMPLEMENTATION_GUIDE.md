# Mawasem Feature Implementation Guide

## 1. Before Coding

Complete this checklist:

- [ ] Read `AGENTS.md`, `docs/AI_CONTEXT.md`, `docs/ARCHITECTURE.md`, and
  `docs/CODING_RULES.md`.
- [ ] Identify the closest routed feature, not merely a similarly named file.
- [ ] Start with Categories for standard CRUD, Collections for a required
  relation, Seasons for debounced search/extra filters, or Customers for a
  state transition such as block/unblock.
- [ ] Inspect the matching backend controller under
  `server/Mawasem.API/Controllers`.
- [ ] Inspect Application request and response records under
  `server/Mawasem.Application/Features/<Feature>/Contracts`.
- [ ] Confirm HTTP methods, full controller route, status codes, permissions,
  request body, response body, query defaults/limits, and soft-delete behavior.
- [ ] Confirm the dashboard API base URL convention in
  `client/dashboard/src/lib/axios.ts` and the environment variable name. Do not
  copy secret/environment values.
- [ ] Search for existing API functions, Query hooks, shared types, translations,
  UI wrappers, table pieces, and confirmation dialogs.
- [ ] Confirm whether the feature needs a routed details page. Existing details
  hooks alone do not imply one.
- [ ] Confirm both Arabic and English backend fields and UI requirements.
- [ ] For Employees, Roles, Delivery Areas, authentication, or migration work,
  read the matching verified contract section in `docs/AI_CONTEXT.md`.

## 2. Suggested Feature Structure

Use this verified Categories-shaped structure for a full dashboard CRUD feature:

```text
client/dashboard/src/features/widgets/
|-- api/
|   |-- create-widget.ts
|   |-- delete-widget.ts
|   |-- get-widget.ts
|   |-- get-widgets.ts
|   |-- restore-widget.ts
|   `-- update-widget.ts
|-- components/
|   |-- types.ts
|   |-- widget-actions.tsx
|   |-- widget-columns.tsx
|   |-- widget-dialog.tsx
|   `-- widget-form.tsx
|-- hooks/
|   |-- use-create-widget.ts
|   |-- use-delete-widget.ts
|   |-- use-restore-widget.ts
|   |-- use-update-widget.ts
|   |-- use-widget.ts
|   `-- use-widgets.ts
|-- pages/
|   `-- WidgetsPage.tsx
|-- schema/
|   `-- widget-form-schema.ts
`-- types/
    |-- widget-query-params.ts
    `-- widget.ts
```

This is a template, not a requirement to create unused files. Omit operations
the backend does not support. For a compact feature, a single `types.ts` is also
established by Collections and Seasons. Do not create barrel files unless the
existing local feature uses them.

## 3. Implementation Order

1. **Types**: model the verified entity, list response, payload, query params,
   and combined update params.
2. **API functions**: create one file per supported controller operation using
   the configured Axios instance.
3. **TanStack Query hooks**: add list/details queries and supported mutations;
   define query keys and invalidation.
4. **Form schema**: add translated Zod validation, inferred form values, and
   create defaults.
5. **Form**: compose shared form primitives, edit reset behavior, relation
   inputs, and error rendering.
6. **Dialog**: select create/update mutation, mode text, pending/error state,
   success close behavior, and form ID.
7. **Actions**: add edit/delete/restore state and confirmation integration.
8. **Columns**: add translated headers, localized backend values, badges, and
   row actions.
9. **Page**: own search/filter/page/create-dialog state and compose the shared
   management components.
10. **Translation keys**: add complete English and Arabic feature files and
    register them in i18n. In practice, add keys alongside the UI that consumes
    them so hardcoded placeholders never enter the feature.
11. **Route/sidebar integration**: add the page to
    `src/routes/dashboard.routes.tsx` and navigation to `src/lib/data.ts`, with
    matching `sidebar.*` keys in both languages.
12. **Verification**: run checks, inspect both directions, test flows, and
    review the diff for unrelated changes.

If the backend endpoint does not exist, stop and clarify scope rather than
inventing a frontend contract.

## 4. API Checklist

For every endpoint, record and verify:

- [ ] Controller route prefix (dashboard catalog controllers use
  `api/admin/<entities>`).
- [ ] HTTP method.
- [ ] Relative frontend URL after the Axios base URL is applied.
- [ ] Path parameter name/type.
- [ ] Query parameter names, optionality, defaults, and maximums.
- [ ] Request body fields and nullability.
- [ ] Response body casing and field types.
- [ ] Success status code and whether a body exists.
- [ ] Problem/error response behavior relevant to the UI.
- [ ] Required backend permission.
- [ ] Soft-delete, restore, or other state-transition semantics.

For the established catalog CRUD controllers:

| Operation | Method/relative path | Typical success |
| --- | --- | --- |
| List | `GET /widgets` | 200 + paginated body |
| Details | `GET /widgets/{id}` | 200 + entity |
| Create | `POST /widgets` | 201 + entity |
| Update | `PUT /widgets/{id}` | 200 + entity |
| Delete | `DELETE /widgets/{id}` | 204, soft-delete |
| Restore | `POST /widgets/{id}/restore` | 204 |

This table applies only after the target controller confirms it.

Use Axios `params` for list inputs:

```ts
api.get<PaginatedResponse<Widget>>("/widgets", {
  params: {
    search,
    includeDeleted,
    pageNumber,
    pageSize,
  },
})
```

The checked dashboard base path already contains `/api/admin`. Never produce
`/api/admin/admin/widgets`.

## 5. Hook Checklist

- [ ] List key is plural and includes the complete params object.
- [ ] Details key is singular and includes the ID.
- [ ] Details query has an appropriate `enabled` condition.
- [ ] Query function calls exactly one API function.
- [ ] Mutation function matches the API function's input type.
- [ ] Successful mutations invalidate the plural list-key prefix.
- [ ] Returned data naming is consistent inside the feature.
- [ ] Pending state is exposed (`isPending` or the established `isLoading`
  alias).
- [ ] Error is exposed to the component/dialog that renders it.
- [ ] `mutateAsync` is exposed when a dialog must await success before closing.
- [ ] No JSX, translations, or UI state exists in the hook.

Use Categories as the explicit-return reference:

```ts
const {
  mutateAsync: createWidgetMutationAsync,
  isPending: isLoading,
  error,
} = useMutation({
  mutationFn: createWidget,
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ["widgets"] })
  },
})
```

## 6. UI Checklist

- [ ] Translated page title and description.
- [ ] Controlled search with page reset.
- [ ] Search normalization/debounce if the domain needs it.
- [ ] Verified filters, each resetting page 1.
- [ ] `EntityTable` or `EntityManagementPage`, not a duplicate table.
- [ ] Loading state.
- [ ] Rendered request error.
- [ ] Translated empty state.
- [ ] `EntityPagination` with backend totals and boundary guards.
- [ ] Create action only when the backend/permission model supports it.
- [ ] Edit action only for active editable rows.
- [ ] Delete action only for active soft-deletable rows.
- [ ] Restore action only for deleted rows.
- [ ] Correct status badge for active/inactive/deleted/blocked states.
- [ ] Localized backend names/descriptions where appropriate.
- [ ] Translated accessible action labels.
- [ ] Pending actions disabled.
- [ ] Both RTL and LTR checked.
- [ ] Translation coverage in both language files.

Pass every label into shared components. Their English defaults are fallback
behavior, not permission to leave a feature untranslated.

## 7. Form Checklist

- [ ] Create defaults match payload types.
- [ ] Edit defaults are derived from the selected entity.
- [ ] `form.reset` runs when entity/mode changes.
- [ ] Zod schema validates every required field.
- [ ] Validation messages use the active translation function.
- [ ] HTML form ID matches `EntityDialogFooter.formId`.
- [ ] Dialog chooses create or update mutation by mode.
- [ ] Submit and cancel are disabled while pending.
- [ ] Dialog closes after success.
- [ ] Dialog stays open and displays the API/mutation error after failure.
- [ ] Required relations load only valid options.
- [ ] Relation option labels select Arabic/English backend fields.
- [ ] DOM relation values are converted to numeric IDs.
- [ ] Relation requests respect backend page-size limits and pagination.
- [ ] No backend entity content is copied into translation JSON.

For a relation, inspect `collection-form.tsx` for UI/data-flow structure and
reuse `CATALOGUE_OPTIONS_PAGE_SIZE`. The catalogue backend maximum is 100.

## 8. Contract-Specific Checklists

### Employees

- [ ] Use relative dashboard paths such as `/employees` and
  `/employees/{employeeId}`.
- [ ] Fetch role and direct-permission choices from
  `/employees/access-options`.
- [ ] Initialize role selection from `roles`.
- [ ] Initialize direct-permission selection from `directPermissions`, not
  `effectivePermissions`.
- [ ] Submit `{ roleNames }` to `/roles` and `{ permissionNames }` to
  `/permissions`.
- [ ] Treat effective permissions as a count/summary unless a dedicated
  read-only view needs the names.
- [ ] Use the single-employee endpoint in the details dialog with
  `enabled: open && employeeId > 0`.
- [ ] Keep temporary-password values only in form state; clear them when the
  dialog closes and never place them in Query data/cache or browser storage.
- [ ] Treat block, unblock, reset-password, and other successful no-body
  operations as `204`.

### Roles

- [ ] Use `/roles/permission-options`, never employee access options.
- [ ] Honor `isRequired` for `Dashboard.Access`.
- [ ] Honor role response `isProtected` and `canManagePermissions`.
- [ ] Submit only `{ permissionNames }` to
  `/roles/{roleName}/permissions`.
- [ ] Show a compact permission count in the table.
- [ ] Use a searchable, grouped, scrollable dialog when the permission set is
  large.
- [ ] Gate permission-options fetching to the dialog open state with an
  `enabled` condition.

### Delivery Areas

- [ ] Model Pending `1`, Confirmed `2`, and Restricted `3`.
- [ ] Support list `search`, `status`, `isActive`, `includeDeleted`,
  `pageNumber`, and `pageSize`.
- [ ] Keep `deliveryFee`, `effectiveDeliveryFee`, `isFreeDelivery`,
  `isActive`, `status`, `activeAddressCount`, and `isDeleted` distinct.
- [ ] Send status only to `/delivery-areas/{id}/status`.
- [ ] Send the ordinary field payload to `/delivery-areas/{id}` without a
  status field.
- [ ] Treat delete and restore as `204`.
- [ ] Map `delivery_areas.has_active_addresses` to an actionable conflict
  message.
- [ ] Keep a restore conflict dialog open for
  `delivery_areas.duplicate_name`.
- [ ] Use `DeliveryAreas.Delete` as the verified restore permission until the
  backend changes.

### Collections and database diagnostics

- [ ] Treat `seasonId` as required for collection create/update.
- [ ] Load only non-deleted Seasons; inactive non-deleted Seasons are accepted
  by the current backend.
- [ ] If SQL Server reports `Invalid column name 'SeasonId'`, inspect
  migration `20260721201331_AddSeasonToCollections` and the target
  `__EFMigrationsHistory` before changing code.
- [ ] From `server`, use:

  ```text
  dotnet ef database update --project Mawasem.Infrastructure --startup-project Mawasem.API
  ```

  only with the intended locally configured database. Never paste its
  connection value into documentation or logs.

## 9. Example CRUD Flow

Project-specific pseudocode for a standard soft-deletable catalog feature:

```text
WidgetsPage
  state: search, includeDeleted, requestedPageNumber, createDialogOpen
  data: useWidgets({
    search,
    includeDeleted,
    pageNumber: requestedPageNumber,
    pageSize: 10
  })
  columns: useWidgetColumns()
  render EntityManagementPage + WidgetDialog(mode="create")

useWidgets(params)
  useQuery({
    queryKey: ["widgets", params],
    queryFn: () => getWidgets(params)
  })

WidgetDialog(mode, widget?)
  createMutation = useCreateWidget()
  updateMutation = useUpdateWidget()
  submit(values):
    if edit: await updateMutation({ id: widget.id, data: values })
    else: await createMutation(values)
    close only after success

WidgetActions(widget)
  if widget.isDeleted:
    Restore -> confirmation -> useRestoreWidget()
  else:
    Edit -> WidgetDialog(mode="edit")
    Delete -> confirmation -> useDeleteWidget()

Every successful mutation
  invalidateQueries({ queryKey: ["widgets"] })
  active list refetches
```

## 10. Feature Completion Definition

A dashboard feature is complete only when all applicable items are true:

- [ ] `npm run typecheck` passes in `client/dashboard`.
- [ ] `npm run lint` passes in `client/dashboard`.
- [ ] `npm run build` passes in `client/dashboard`.
- [ ] API calls use the verified relative path and do not duplicate admin
  prefixes.
- [ ] Search, filters, page reset, and pagination work against the backend.
- [ ] Create works when supported.
- [ ] Edit works when supported.
- [ ] Delete works when supported.
- [ ] Restore works where soft delete is supported.
- [ ] Alternative state transitions (for example block/unblock) work where
  applicable.
- [ ] Loading, error, empty, pending, and failure-retention states work.
- [ ] All static strings have matching English and Arabic keys.
- [ ] Localized backend fields switch with the language.
- [ ] RTL and LTR layouts have been checked.
- [ ] No shared table/dialog/pagination/confirmation functionality was
  duplicated.
- [ ] The diff contains no unrelated changes or secret values.
- [ ] Documentation is updated if the feature introduced a new pattern,
  abstraction, folder/API convention, or global behavior.

There is no frontend automated test script at present. Backend changes also
require the relevant `Mawasem.Tests` coverage and .NET test execution.

## 11. Current Reference Matrix

| Requirement | Reference feature | Reference path | Notes |
| --- | --- | --- | --- |
| Full localized CRUD composition | Categories | `client/dashboard/src/features/categories` | Primary reference |
| Entity/payload/pagination types | Categories | `client/dashboard/src/features/categories/types/category.ts` | Uses shared `PaginatedResponse<T>` |
| Query parameter type | Categories | `client/dashboard/src/features/categories/types/category-query-params.ts` | Required page number/size |
| One-operation API layout | Categories | `client/dashboard/src/features/categories/api` | Paths match plural backend route |
| List Query hook | Categories | `client/dashboard/src/features/categories/hooks/use-categories.ts` | `["categories", params]` |
| Details Query hook | Categories | `client/dashboard/src/features/categories/hooks/use-category.ts` | `enabled: !!id`; no details page yet |
| Explicit mutation hook result | Categories | `client/dashboard/src/features/categories/hooks/use-create-category.ts` | Exposes sync/async mutations, loading, error |
| Translated Zod schema | Categories | `client/dashboard/src/features/categories/schema/category-form-schema.ts` | Schema factory receives `t` |
| Create/edit form reset | Categories | `client/dashboard/src/features/categories/components/category-form.tsx` | Resets when mode/entity changes |
| Create/edit dialog orchestration | Categories | `client/dashboard/src/features/categories/components/category-dialog.tsx` | Close success, retain failure |
| Delete/restore actions | Categories | `client/dashboard/src/features/categories/components/category-actions.tsx` | Conditional on `isDeleted` |
| Localized/memoized columns | Categories | `client/dashboard/src/features/categories/components/category-columns.tsx` | Domain columns correctly feature-local |
| Standard management page | Categories | `client/dashboard/src/features/categories/pages/CategoriesPage.tsx` | Uses shared page composition |
| Relation-backed form | Collections | `client/dashboard/src/features/collections/components/collection-form.tsx` | Season relation; uses the shared 100-row catalogue option limit |
| Localized relation column | Collections | `client/dashboard/src/features/collections/components/collection-columns.tsx` | Selects season Arabic/English name |
| Debounced normalized search | Seasons | `client/dashboard/src/features/seasons/pages/SeasonsPage.tsx` | `normalizeArabic` + 500 ms debounce |
| Extra list filter slot | Seasons | `client/dashboard/src/features/seasons/pages/SeasonsPage.tsx` | Active-only switch through `filtersSlot` |
| Active/inactive/deleted badge | Seasons | `client/dashboard/src/features/seasons/components/season-columns.tsx` | Also contains legacy `IsDeleted` compatibility |
| Non-CRUD state transition form | Customers | `client/dashboard/src/features/customers/components/block-customer-dialog.tsx` | Block reason with translated validation |
| Callback confirmation variant | Customers | `client/dashboard/src/features/customers/components/unblock-customer-dialog.tsx` | Uses older `DeleteEntityDialog` |
| Generic management composition | Shared | `client/dashboard/src/components/entity-management/EntityManagementPage.tsx` | No domain imports |
| Generic table/toolbar/pagination | Shared | `client/dashboard/src/components/entity-table` | Use translated optional labels |
| Create/edit dialog and footer | Shared | `client/dashboard/src/components/entity-dialog/entity-dialog.tsx` | See footer and shared types beside it |
| Mutation-backed confirmations | Shared | `client/dashboard/src/components/entity-dialog/EntityDeleteDialog.tsx` | Paired with `EntityRestoreDialog.tsx` |
| Axios configuration | Shared | `client/dashboard/src/lib/axios.ts` | `VITE_API_URL`, credentials enabled |
| Query defaults | Shared | `client/dashboard/src/lib/react-query.ts` | One-minute stale time, one retry |
| i18n registration/direction | Shared | `client/dashboard/src/lib/i18n/index.ts` | Flat keys merged into one namespace |
| Routing | Shared | `client/dashboard/src/routes/dashboard.routes.tsx` | Add routed page under `AdminLayout` |
| Sidebar integration | Shared | `client/dashboard/src/lib/data.ts` | Add matching bilingual `sidebar.*` key |
| Backend CRUD route/status | Categories backend | `server/Mawasem.API/Controllers/AdminCategoriesController.cs` | Controller is contract source of truth |
| Backend pagination response | Categories backend | `server/Mawasem.Application/Features/Categories/Contracts/Responses/CategoryListResponse.cs` | Mirrors frontend shared shape |
| Collection/Season validity | Collections backend | `server/Mawasem.Infrastructure/Collections/CollectionManagementService.Helpers.cs` | Relation must reference a non-deleted Season |
| Employee contracts and access options | Employees backend | `server/Mawasem.API/Controllers/AdminEmployeesController.cs` | Access options differ from role permission options |
| Employee access restrictions | Employees backend | `server/Mawasem.Infrastructure/Employees/EmployeeManagementService.Helpers.cs` | Actor-scoped roles and direct permissions |
| Employee password reset | Employees backend | `server/Mawasem.Infrastructure/Employees/EmployeeManagementService.AccountSecurity.cs` | Must-change, lockout clearing, token revocation |
| Role permission rules | Roles backend | `server/Mawasem.Infrastructure/Roles/RolePermissionManagementService.cs` | Protected roles and required dashboard access |
| Delivery Area CRUD and transitions | Delivery Areas backend | `server/Mawasem.API/Controllers/AdminDeliveryAreasController.cs` | Separate status endpoint; delete permission restores |
| Delivery Area business rules | Delivery Areas backend | `server/Mawasem.Infrastructure/DeliveryAreas/DeliveryAreaService.Mutations.cs` | Fee normalization and delete/restore conflicts |
| Collection Season migration | EF Core migration | `server/Mawasem.Infrastructure/Persistence/Migrations/20260721201331_AddSeasonToCollections.cs` | Adds required `SeasonId` relationship |

Do not use Brands as the general reference until its hardcoded strings, misplaced
columns, and singular details endpoint are reconciled with the verified
architecture and backend.
