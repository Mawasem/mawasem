# Mawasem Architecture

## 1. Architectural Style

The active dashboard uses a feature-based React architecture layered over
shared UI and infrastructure modules:

- `features/<domain>` owns domain types, endpoint wrappers, TanStack Query
  hooks, schemas, components, and routed pages.
- `components` owns domain-agnostic composition and shadcn/Radix primitives.
- `lib` owns cross-cutting runtime configuration such as Axios, TanStack Query,
  i18n, normalization, and navigation data.
- `routes` connects feature pages to React Router layouts and guards.

This is not a formal frontend Clean Architecture implementation. There are no
repository/use-case layers in the React clients. API functions call Axios
directly, Query hooks call those functions, and feature components orchestrate
the hooks.

The backend has a separate concrete layered structure:

| Project | Verified responsibility |
| --- | --- |
| `Mawasem.Domain` | entities, value objects, enums, and system role/permission definitions |
| `Mawasem.Application` | HTTP-facing contracts, feature interfaces, result/error models, and options |
| `Mawasem.Infrastructure` | service implementations, Identity operations, EF Core context/configurations/migrations, persistence, and storage |
| `Mawasem.API` | startup/DI, controllers, authentication cookies and JWT validation, permission policies, and HTTP error mapping |
| `Mawasem.Tests` | backend tests across services, authorization, authentication, workflows, and API integration |

Application references Domain. Infrastructure references Application and
Domain. API is the startup/composition project and references all three.
Tests reference all backend projects. This is the observed repository
architecture, not a claim that every textbook Clean Architecture rule is
implemented.

The verified backend request flow is:

```text
Controller
  -> Application contract/interface
  -> Infrastructure service
  -> EF Core and/or ASP.NET Core Identity
  -> SQL Server
```

Frontend work should derive routes and status codes from controllers,
request/response shapes from Application contracts, and business restrictions
from Infrastructure services. Database entities alone are not API contracts.

## 2. Dependency Direction

The established frontend dependency direction is:

```text
shared UI <- feature components <- feature pages <- route configuration

Axios instance <- API functions <- TanStack Query hooks <- UI orchestration
```

Allowed imports:

- Shared components may import shared UI, shared types, utilities, and React
  libraries.
- Feature API files may import the configured Axios instance and feature/shared
  types.
- Feature hooks may import API functions, feature types, and TanStack Query.
- Feature components may import their feature's hooks/types/schema/components
  and shared components.
- Pages may import their feature components/hooks and shared page composition.
- Routes may import layouts, guards, and pages.

Avoid these inverse dependencies:

- Shared components must not import feature types, hooks, translations, or API
  files.
- API files must not import React, hooks, forms, or UI.
- Hooks must not render JSX or control dialogs.
- Column definitions must not issue network requests.
- Generic types must not depend on a particular business domain.

There is one known violation:
`components/entity-table/columns/brand-columns.tsx` imports Brands domain code.
Do not reproduce it; feature-aware columns belong under the feature.

## 3. Feature Anatomy

Categories is the clearest complete structure:

```text
client/dashboard/src/features/categories/
|-- api/
|   |-- create-category.ts
|   |-- delete-category.ts
|   |-- get-categories.ts
|   |-- get-category.ts
|   |-- restore-category.ts
|   `-- update-category.ts
|-- components/
|   |-- category-actions.tsx
|   |-- category-columns.tsx
|   |-- category-dialog.tsx
|   |-- category-form.tsx
|   `-- types.ts
|-- hooks/
|   |-- use-categories.ts
|   |-- use-category.ts
|   |-- use-create-category.ts
|   |-- use-delete-category.ts
|   |-- use-restore-category.ts
|   `-- use-update-category.ts
|-- pages/
|   `-- CategoriesPage.tsx
|-- schema/
|   `-- category-form-schema.ts
`-- types/
    |-- category-query-params.ts
    `-- category.ts
```

Responsibilities:

- `api/`: one HTTP operation per file.
- `hooks/`: one Query or mutation hook per file.
- `components/`: forms, dialogs, actions, columns, and their private prop types.
- `pages/`: route-level orchestration and list state.
- `schema/`: Zod schema factory, inferred form type, and defaults.
- `types.ts` or `types/`: entity, payload, operation-param, query-param, and
  response types.

The exact type layout varies: Categories and Brands use a `types/` directory;
Collections, Seasons, and Customers use a feature-level `types.ts`. Do not move
existing types solely for uniformity. For a new feature, choose one feature
type file unless the domain has enough distinct contracts to justify a folder.

## 4. API Layer

Dashboard API files follow these verified rules:

- One operation per file.
- File names are kebab-case verb/entity names.
- Exported functions use camelCase verbs: `getCategories`, `createCategory`,
  `updateCategory`, `deleteCategory`, `restoreCategory`.
- Use `api` from `@/lib/axios`.
- Use feature request/response types and `PaginatedResponse<T>` when applicable.
- Pass query parameters through Axios `params`.
- Return `response.data` for response-bearing operations.
- Let Axios errors propagate to TanStack Query; API functions do not catch them.
- Do not include React hooks or UI logic.

Verified list pattern:

```ts
export async function getCategories(params: CategoryQueryParams) {
  const response = await api.get<PaginatedResponse<Category>>(
    "/categories",
    { params }
  )

  return response.data
}
```

The actual Categories implementation destructures params before passing them;
both forms preserve Axios parameter serialization. Do not manually build a
query string.

Verified update pattern:

```ts
export async function updateCategory({
  id,
  data,
}: UpdateCategoryParams) {
  const response = await api.put<Category>(`/categories/${id}`, data)
  return response.data
}
```

Delete/restore controllers return 204. Their frontend functions may simply
await the call without a response type. Existing Category/Brand restore
wrappers type a response even though the checked controller returns no content;
new code should follow the actual controller contract.

All dashboard paths are relative to `VITE_API_URL`, whose checked-in path is
`/api/admin`. API files use `/categories`, not `/api/admin/categories`.

## 5. React Query Layer

`client/dashboard/src/lib/react-query.ts` creates one `QueryClient` with a
one-minute query stale time and one retry by default. `useMe` overrides retry to
false. The provider is installed in `client/dashboard/src/main.tsx`.

### Query keys

The observed conventions are:

- List: plural domain plus the whole params object, e.g.
  `["categories", params]`.
- Details: singular domain plus ID, e.g. `["category", id]`.
- Auth profile: `["me"]`.
- Successful mutations invalidate the plural prefix, e.g.
  `invalidateQueries({ queryKey: ["categories"] })`, which covers every active
  list parameter variant.

There is no central query-key factory. Do not introduce one for a single
feature; if a shared factory is proposed, update these docs and migrate an
intentional scope.

### List hooks

List hooks wrap `useQuery`, rename `isPending` to `isLoading`, and explicitly
expose data and errors:

```ts
const {
  data,
  isPending: isLoading,
  error,
} = useQuery({
  queryKey: ["categories", params],
  queryFn: () => getCategories(params),
})

return { data, isLoading, error }
```

Customers uses `customersData`, while Categories/Collections/Seasons/Brands
usually return `data`. Follow the closest reference consistently within the
feature.

### Details hooks

Details hooks add the entity ID to the key and prevent a request for a falsy
numeric ID:

```ts
useQuery({
  queryKey: ["category", id],
  queryFn: () => getCategory(id),
  enabled: !!id,
})
```

Existing details hooks rename data to `category`, `collectionData`,
`customerData`, `brand`, or `season`.

### Mutation hooks

Every mutation hook:

1. obtains `useQueryClient()`;
2. calls exactly one API function through `mutationFn`;
3. invalidates the plural list key on success;
4. exposes pending/error state to UI.

Two return styles coexist. Brands and most Seasons mutations return the raw
`useMutation` result; Categories and Collections return explicitly renamed
properties. Categories is the primary naming reference:

```ts
return {
  updateCategoryMutation,
  updateCategoryMutationAsync,
  isLoading,
  error,
}
```

## 6. Page Layer

Catalog list pages own:

- controlled search input;
- optional search normalization/debouncing;
- filter state such as `includeDeleted` or `isActive`;
- requested page number and fixed page size;
- create-dialog open state;
- page-boundary checks;
- translation lookup;
- the list hook and column hook;
- assembly of `EntityManagementPage`.

Search/filter changes reset page number to 1. Pages derive current page,
`totalPages`, and `totalCount` from the response with safe fallbacks. Seasons
and Brands normalize Arabic text and debounce it for 500 ms;
Categories/Collections currently request on every controlled search change.

Pages should not:

- call Axios directly;
- define backend response interfaces;
- execute delete/restore mutations;
- define domain form schemas;
- reimplement generic tables, pagination, or dialog shells.

Customers does not have a create action or include-deleted filter, so
`CustomerPage.tsx` composes `EntityTable` and `EntityPagination` directly.
Roles also composes those primitives directly because it is a fixed,
non-paginated system-role list rather than catalog CRUD. Do not force
`EntityManagementPage` into either mismatched workflow.

## 7. Columns Layer

Domain column definitions belong in
`features/<feature>/components/<feature>-columns.tsx`. Because headers, status
text, localized backend values, and row actions depend on hooks, implemented
features expose a hook such as `useCategoryColumns()` and memoize the returned
`ColumnDef<Entity>[]`.

Columns may:

- use `useTranslation`;
- select a backend Arabic or English field from `i18n.resolvedLanguage`;
- render `Badge` variants for current domain status;
- format display-only values such as customer totals;
- render the feature action component for `row.original`.

Columns must not call APIs or own mutation/dialog state. The action component
owns those concerns. Include every value used from i18n in the `useMemo`
dependency list.

The Brands column location under shared components is a known exception, not a
template.

## 8. Actions Layer

An action component receives one row entity and owns:

- dropdown rendering;
- edit/delete/restore dialog open state;
- the feature's delete and restore mutation hooks;
- localized entity-name selection;
- conditional actions based on status.

Catalog behavior is:

```text
isDeleted = false -> Edit + Delete
isDeleted = true  -> Restore only
```

The dropdown does not execute a destructive mutation directly. It opens a
confirmation dialog and passes an adapter with `mutateAsync`, pending state,
and error. Customers uses the analogous active -> Block and blocked -> Unblock
flow with feature-specific dialogs.

## 9. Dialog Layer

Feature create/edit dialogs orchestrate rather than render fields. A catalog
dialog:

1. receives controlled open state, mode, and optional entity;
2. creates both create and update mutation hooks;
3. selects translated title and description by mode;
4. derives a stable form ID such as `category-form-create`;
5. combines mutation pending/error state;
6. awaits the correct mutation in `handleSubmit`;
7. closes only on success;
8. catches rejection so the dialog remains open and its mutation error remains
   visible;
9. composes the feature form and shared `EntityDialogFooter`.

Delete/restore dialogs follow the same success/failure behavior internally.

Dialogs should not duplicate field markup or bypass the feature hook to call an
API function.

## 10. Form Layer

Catalog forms use:

- `useForm<FormValues>()`;
- `zodResolver` from `@hookform/resolvers/zod`;
- a feature Zod schema;
- shadcn `Form` wrappers and inputs;
- a passed async `onSubmit`;
- a visible API error area.

Localized validation schemas are factories that receive `t`, as shown by
Categories, Collections, Seasons, and customer blocking. Brands currently uses
hardcoded validation text and is not the localization reference.

Schemas own inferred `FormValues` and create defaults. Forms build edit
defaults from the passed entity, then use `useEffect` and `form.reset(...)`
whenever the entity or mode changes. This prevents a reused dialog from showing
stale values.

The HTML `<form id={formId}>` lives in the form component. The submit button
lives outside it in `EntityDialogFooter`, so its `form={formId}` link is
required.

Relation IDs are numbers in schemas and payloads. `CollectionForm` converts the
native select string with `Number(nextValue)` and uses `0` as the unselected
invalid value. It displays `season.nameAr` or `season.nameEn` according to the
current language.

Collections depend on Seasons:

```text
CollectionForm
  -> useSeasons({ includeDeleted: false, ... })
  -> non-deleted Season options
  -> numeric seasonId in CollectionFormValues
```

Catalogue option loaders reuse `CATALOGUE_OPTIONS_PAGE_SIZE` from
`client/dashboard/src/lib/catalogue-options.ts`. Its value is the backend
maximum of 100. Relation pickers that need more than the first 100 records must
resolve pagination or search instead of increasing that value.

## 11. Shared Versus Feature Components

Use this decision guide:

| Question | Shared component | Feature component |
| --- | --- | --- |
| Knows a domain entity or payload? | no | yes |
| Imports feature hooks? | no | allowed |
| Calls translations for a named domain? | no | allowed |
| Reused unchanged by multiple domains? | yes | not required |
| Receives behavior/data through props? | yes | may orchestrate behavior |
| Renders domain fields/status/actions? | no | yes |

Examples:

- Shared: `EntityTable`, `EntityPagination`, `EntityDialog`,
  `EntityDialogFooter`.
- Feature: `CategoryForm`, `CollectionActions`, `useSeasonColumns`.

Do not generalize a one-off component merely because it could theoretically be
reused. Extract only demonstrated, domain-agnostic repetition.

## 12. Architectural Flow Diagrams

List flow:

```text
CategoriesPage
  |
  v
useCategories(params)
  |
  v
getCategories(params)
  |
  v
api (Axios, VITE_API_URL + cookies)
  |
  v
AdminCategoriesController
  |
  v
CategoryManagementService -> EF Core -> SQL Server
```

Mutation flow:

```text
CategoryActions / CategoryDialog
  |
  v
useDeleteCategory / useCreateCategory / useUpdateCategory
  |
  v
single API function
  |
  v
admin controller
  |
  v
invalidateQueries(["categories"])
  |
  v
active table query refetches
```

Form flow:

```text
CategoryDialog
  |
  +--> CategoryForm
  |      |
  |      v
  |    React Hook Form + translated Zod schema
  |
  +--> EntityDialogFooter -- form ID --> CategoryForm submit
  |
  v
create/update mutation -> close on success
                       -> keep open and show error on failure
```

Backend employee authorization flow:

```text
JWT access-token cookie
  -> JWT bearer authentication
  -> RequirePermission policy
  -> PermissionAuthorizationHandler
       -> current ApplicationUser state
       -> dashboard role assignments
       -> role permissions + direct user permissions
  -> controller action
  -> IEmployeeManagementService
  -> EmployeeManagementService
  -> Identity + MawasemDbContext
  -> SQL Server
```

The handler queries current database state and rejects blocked employees,
employees requiring a password change, users without a dashboard role, deleted
permissions, and missing permissions. Customer-only roles remain isolated from
dashboard permission authorization.

Authentication has two controller/service flows:

```text
Dashboard: /api/admin/auth -> IDashboardAuthenticationService
Customer:  /api/auth       -> ICustomerAuthenticationService
```

They share the access-token cookie name and JWT bearer validation, but use
separate path-scoped refresh-token cookies and separate services. Serialized
session responses do not expose token values.

## 13. Contract-Sensitive Feature Architecture

### Employees

Employee list/detail responses expose both `directPermissions` and
`effectivePermissions`. The latter is the distinct union of direct and
role-derived permissions and is display-only. Employee assignment UI must:

- fetch roles and directly assignable permissions from
  `GET /api/admin/employees/access-options`;
- initialize the direct-permission selection from `directPermissions`;
- submit only direct permission names to
  `PUT /api/admin/employees/{employeeId}/permissions`;
- use the single-employee endpoint for details dialogs;
- gate dialog queries with an `enabled` condition while closed.

Role `permission-options` is a different contract. It can include
`Dashboard.Access` marked required when the actor may assign it; employee
access options intentionally exclude that permission.

### Roles

Roles are fixed system roles returned by the backend, not dashboard-created
entities. `SuperAdmin` and `Customer` are protected. Editable dashboard roles
must retain `Dashboard.Access`. The role list response itself supplies
`isProtected` and `canManagePermissions`; the UI should honor those fields
instead of recreating authorization rules from role-name strings.

### Delivery Areas

Delivery Areas separates ordinary field updates from status transitions:

```text
PUT /delivery-areas/{id}
  -> name, fee, free-delivery flag, active flag

PUT /delivery-areas/{id}/status
  -> DeliveryAreaStatus only
```

The management response deliberately carries both stored `deliveryFee` and
computed `effectiveDeliveryFee`, plus `isFreeDelivery`, `isActive`, `status`,
`activeAddressCount`, and `isDeleted`. These values are not interchangeable in
the UI. Public delivery-area queries add the invariant: non-deleted, active,
and Confirmed.

### Collections and database schema

Collections have a required many-to-one relationship to Seasons through
`SeasonId`, configured with restrictive deletion. The relationship exists in
source only after migration `20260721201331_AddSeasonToCollections` is applied
to the target database. Runtime schema errors must be diagnosed against
`__EFMigrationsHistory`; changing correct entity code is not a schema-update
strategy.
