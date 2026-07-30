# Mawasem AI Project Context

## 1. Project Overview

Mawasem is a bilingual Arabic/English e-commerce system. The repository contains
three applications:

- `client/dashboard`: the active React admin dashboard.
- `client/store`: a customer storefront scaffold.
- `server`: an ASP.NET Core API and its application, domain, infrastructure,
  and test projects.

The dashboard currently authenticates dashboard employees and exposes routed
management UI for brands, categories, collections, seasons, customers,
employees, and roles. Its catalog features support server-side search and
pagination. Brands, categories, collections, and seasons implement create,
edit, soft-delete, and restore. Customers use block/unblock operations.
Employees support profile editing, block/unblock, role and direct-permission
assignment, and administrative password reset. Roles expose permission
management. Delivery Areas has backend APIs but no routed dashboard feature.

The storefront currently has login/signup UI routes and a coming-soon home
image. Its forms are static scaffolds and it does not yet wire Axios, TanStack
Query, or i18n in `src`. The intended production storefront scope beyond the
existing backend public APIs is **Needs verification**.

Confirmed backend domains include dashboard and customer authentication,
catalog (brands, categories, collections, seasons, products, options, variants,
and images), customers and employees, roles/permissions, carts, checkout,
orders, refunds, delivery areas/addresses, and public catalog queries. Not all
backend domains have dashboard pages.

## 2. Technology Stack

### Frontend

Both clients are Vite React TypeScript applications. Versions below come from
their `package.json` files; dependency ranges use `^` or `~` in the source.

| Technology | Dashboard | Storefront | Evidence |
| --- | ---: | ---: | --- |
| React / React DOM | 19.2.6 | 19.2.8 | client package files |
| TypeScript | ~6 | ~6 | client package files |
| Vite | ^8 | ^8 | client package files |
| React Router DOM | ^7.18.1 | ^7.18.1 | client package files |
| TanStack Query | ^5.101.2 | dependency only | dashboard is wired in `src/main.tsx` |
| Axios | ^1.18.1 | dependency only | dashboard uses `src/lib/axios.ts` |
| React Hook Form | ^7.82.0 | installed | dashboard forms use it |
| Zod | ^4.4.3 | installed | dashboard schemas use it |
| Tailwind CSS | ^4 | ^4 | Vite plugins and `src/index.css` |
| shadcn/ui + Radix | shadcn ^4.13.1 | shadcn ^4.13.1 | `components.json`, `components/ui` |
| i18next / react-i18next | ^26.3.6 / ^17.0.10 | not installed | dashboard only |
| TanStack Table | ^8.21.3 | not installed | dashboard entity table |
| Icons | Lucide plus Hugeicons packages | Lucide | actual dashboard feature code mainly uses Lucide |

The dashboard TypeScript config is strict and defines `@/*` as an alias for
`src/*`. The shadcn configuration is RTL-aware. Prettier is configured for LF,
double quotes, no semicolons, two spaces, an 80-column print width, and Tailwind
class sorting.

### Backend

- ASP.NET Core Web API targeting .NET 8:
  `server/Mawasem.API/Mawasem.API.csproj`
- Entity Framework Core 8.0.28 with SQL Server:
  `server/Mawasem.Infrastructure/Mawasem.Infrastructure.csproj`
- ASP.NET Core Identity and cookie-carried JWT authentication
- Swagger/OpenAPI via Swashbuckle 6.6.2
- xUnit test project:
  `server/Mawasem.Tests/Mawasem.Tests.csproj`

The backend projects are separated into Domain, Application contracts and
interfaces, Infrastructure implementations/persistence, API controllers, and
Tests.

## 3. Repository Structure

```text
mawasem/
|-- client/
|   |-- dashboard/
|   |   |-- src/
|   |   |   |-- components/       shared app, entity, and shadcn UI
|   |   |   |-- features/         auth and dashboard domain features
|   |   |   |-- hooks/            global React hooks
|   |   |   |-- layouts/          admin and auth route layouts
|   |   |   |-- lib/              Axios, Query, i18n, utilities, nav data
|   |   |   |-- pages/            dashboard home and not-found pages
|   |   |   |-- routes/           browser-router definitions and guard
|   |   |   `-- types/            cross-feature types such as pagination
|   |   |-- package.json
|   |   `-- vite.config.ts
|   `-- store/
|       |-- src/
|       |   |-- components/       storefront primitives and scaffold forms
|       |   |-- layouts/
|       |   |-- pages/
|       |   `-- routes/
|       |-- package.json
|       `-- vite.config.ts
|-- server/
|   |-- Mawasem.Domain/            entities, value objects, enums, interfaces
|   |-- Mawasem.Application/       feature contracts, interfaces, result models
|   |-- Mawasem.Infrastructure/    EF Core and feature service implementations
|   |-- Mawasem.API/               controllers, auth, authorization, startup
|   |-- Mawasem.Tests/             xUnit service/workflow tests
|   `-- MawasemECommerce.slnx
|-- docs/
`-- AGENTS.md
```

The dashboard uses a feature-based structure. Shared, domain-agnostic building
blocks live in `client/dashboard/src/components`; feature-specific types, API
requests, hooks, forms, actions, columns, and pages live under
`client/dashboard/src/features/<feature>`.

## 4. Implemented Features

### Dashboard routes and UI

| Feature | Path | List | Details | Create | Update | Delete | Restore | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Dashboard auth | `client/dashboard/src/features/auth` | current user | current user | login session | change password | logout session | n/a | `/auth/login` and `/auth/change-password`; cookies are sent via Axios |
| Brands | `client/dashboard/src/features/brands` | yes | API/hook only | yes | yes | soft | yes | `isActive` filter supported by API; UI has known inconsistencies |
| Categories | `client/dashboard/src/features/categories` | yes | API/hook only | yes | yes | soft | yes | primary CRUD reference |
| Collections | `client/dashboard/src/features/collections` | yes | API/hook only | yes | yes | soft | yes | requires a non-deleted Season; inactive is accepted |
| Seasons | `client/dashboard/src/features/seasons` | yes | API/hook only | yes | yes | soft | yes | debounced normalized search, active-only filter |
| Customers | `client/dashboard/src/features/customers` | yes | API/hook only | no | block | no | unblock | UI lists customers and confirms block/unblock |
| Employees | `client/dashboard/src/features/Employees` | yes | details dialog | yes | yes | block | unblock | roles, direct permissions, and password reset are managed in dialogs |
| Roles | `client/dashboard/src/features/roles` | yes | row response | no | permissions | no | no | protected roles are read-only |
| Home | `client/dashboard/src/pages/Home/DashboardPage.tsx` | n/a | n/a | n/a | n/a | n/a | n/a | placeholder text only |

“API/hook only” means the endpoint wrapper and details query hook exist, but no
details route/page uses them. Whether dedicated detail pages are planned is
**Needs verification**.

All four catalog CRUD features call the admin endpoints for:

- `GET /<entities>` with search/filter/pagination query parameters;
- `GET /<entities>/{id}`;
- `POST /<entities>`;
- `PUT /<entities>/{id}`;
- `DELETE /<entities>/{id}` (soft-delete);
- `POST /<entities>/{id}/restore`.

The backend returns `201 Created` for create, `200 OK` for list/details/update,
and `204 No Content` for delete and restore. Confirm the exact controller before
adding an operation. Relevant controllers are
`server/Mawasem.API/Controllers/Admin*Controller.cs`.

Collections are owned by a Season. `CollectionPayload` contains `seasonId`, the
response contains localized season names, and `CollectionForm` loads non-deleted
seasons. The backend rejects a missing/deleted season in
`server/Mawasem.Infrastructure/Collections/CollectionManagementService.Helpers.cs`.

### Storefront

Existing storefront routes are `/`, `/auth/login`, and `/auth/signup`.
`client/store/src/pages/store/HomePage.tsx` renders a coming-soon asset.
`client/store/src/components/login-form.tsx` and `signup-form.tsx` are static
Acme-branded scaffolds with no submit integration. Public backend controllers
exist, but storefront consumption is not implemented in this client.

### Backend-only or not-yet-routed dashboard capabilities

The API contains additional controllers for products, product
options/variants/images, delivery areas, orders/order workflow, refund requests,
carts, checkout, addresses, reports, reviews, and public catalog queries.
Their frontend implementation status must be checked individually; translation
JSON files or sidebar keys alone do not mean a page exists. Employees and Roles
are routed dashboard features.

## 5. Shared Components

### Entity management and tables

`client/dashboard/src/components/entity-management/EntityManagementPage.tsx`

- Responsibility: composes a page heading, `EntityToolbar`, filter area,
  include-deleted switch, `EntityTable`, loading/error messages,
  `EntityPagination`, optional filters, and dialog children.
- Important props: translated page/search/button/filter labels, search and
  include-deleted state callbacks, `columns`, `data`, loading/error state,
  pagination data/callback, `filtersSlot`, and `children`.
- Must not contain: feature hooks, entity types, domain columns, endpoint calls,
  or domain-specific translations.
- Used by: Brands, Categories, Collections, Seasons, and Employees.

`client/dashboard/src/components/entity-table/entity-table.tsx`

- Responsibility: generic TanStack Table rendering from `ColumnDef<TData,
  TValue>[]` and data.
- Important props: `columns`, `data`, optional translated `emptyStateLabel`.
- Must not contain: fetching, pagination state, entity actions, or feature
  translations.
- Used directly by Customers and Roles, and through `EntityManagementPage`.

`client/dashboard/src/components/entity-table/entity-toolbar.tsx`

- Responsibility: controlled search input and create button.
- Important props: `search`, `onSearch`, `buttonText`, `onAdd`, optional
  `searchPlaceholder`.
- Must not contain: debouncing, query parameters, or feature-specific filters.
- Used through `EntityManagementPage`.

`client/dashboard/src/components/entity-table/entity-pagination.tsx`

- Responsibility: display total count/current page and emit bounded previous or
  next page values.
- Important props: `totalCount`, `page`, `totalPages`, `onPageChange`, and
  optional translated labels.
- Must not contain: query state or server requests.
- Used across the routed catalog, customer, employee, and role management
  features.

Prop interfaces for these components live in
`client/dashboard/src/components/entity-table/types.ts`.

### Dialogs and confirmations

`client/dashboard/src/components/entity-dialog/entity-dialog.tsx`

- Responsibility: generic controlled shadcn dialog shell with title,
  description, and children.
- Important props: `open`, `onOpenChange`, `title`, `description`, `children`.
- Must not contain: form schemas, mutation selection, or domain fields.
- Used by catalog create/edit dialogs and customer blocking.

`client/dashboard/src/components/entity-dialog/entity-dialog-footer.tsx`

- Responsibility: cancel and external-form submit controls for create/edit
  modes.
- Important props: `mode`, `formId`, `isLoading`, `onCancel`, translated labels.
- The submit button's `form` attribute must match the form element's `id`.
- Must not contain: mutation calls or domain text.

`client/dashboard/src/components/entity-dialog/EntityDeleteDialog.tsx` and
`EntityRestoreDialog.tsx`

- Responsibility: confirmation UI around a passed mutation object; disable
  actions while pending, close after `mutateAsync` succeeds, and stay open while
  displaying `mutation.error` after failure.
- Important props: controlled open state, translated labels/text, mutation,
  entity ID, and optional localized entity name.
- Must not import feature hooks.
- Used by Brands, Categories, Collections, and Seasons.

`client/dashboard/src/components/entity-dialog/delete-entity-dialog.tsx`

- Responsibility: older generic destructive confirmation controlled by
  `onConfirm`, explicit pending state, and explicit error text.
- Important props: controlled open state, translated text/labels,
  `isDeleting`, `errorMessage`, and async `onConfirm`.
- Must not import a feature mutation hook or decide what the action means.
- Used only by customer unblocking, despite its destructive naming.
- Do not create a third confirmation pattern. Choose the mutation-backed
  `EntityDeleteDialog`/`EntityRestoreDialog` for catalog-style features unless
  the existing feature requires the callback form.

Dialog types live in `client/dashboard/src/components/entity-dialog/types.ts`.
Status badges are not wrapped globally: feature column definitions render the
shared `client/dashboard/src/components/ui/badge.tsx` with domain translations.

### UI primitives

`client/dashboard/src/components/ui` contains the shadcn/Radix primitives.
Feature code composes Button, Input, Textarea, Switch, Badge, DropdownMenu,
Dialog, AlertDialog, Form, Label, Table, and related primitives rather than
reimplementing them.

## 6. Data and Request Flow

List flow:

```text
Feature page
  -> local search/filter/page state
  -> useEntities(params)
  -> getEntities(params)
  -> configured Axios instance
  -> ASP.NET Core admin controller
  -> paginated JSON response
  -> TanStack Query cache
  -> shared table/pagination UI
```

The full params object is part of each list query key, for example
`["categories", params]`. Changing search, filters, or page produces a distinct
cache entry and request.

Mutation flow:

```text
Feature dialog/action
  -> feature mutation hook
  -> one API operation
  -> backend mutation
  -> invalidateQueries({ queryKey: ["categories"] })
  -> active list query refetches
  -> table refreshes
```

Create/edit and confirmation dialogs use `mutateAsync` so they close only after
success. Their `catch` blocks intentionally keep the dialog open; errors exposed
by the mutation hook are rendered inside the dialog.

## 7. API Base URL Convention

`client/dashboard/src/lib/axios.ts` creates the only dashboard Axios instance:

```ts
export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  withCredentials: true,
})
```

The checked-in dashboard `.env` defines `VITE_API_URL` with path `/api/admin`
(the origin/value is intentionally not documented). Therefore dashboard API
files use paths relative to the admin prefix:

```text
/categories
/collections/{id}/restore
/auth/me
```

They must not prepend `/api/admin` or another `/admin`. Doing so would produce
incorrect URLs such as `/api/admin/admin/collections`. Backend controllers
confirm their full routes under `api/admin/...`.

If deployment changes `VITE_API_URL`, re-verify this rule. Storefront/public API
calls should not reuse the admin-prefixed base URL without checking their own
Axios configuration; the storefront has no configured Axios instance yet.

## 8. Pagination Contract

The shared frontend type is
`client/dashboard/src/types/pagination.ts`:

```ts
interface PaginatedResponse<T> {
  items: T[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
}
```

Catalog response contracts use the same shape, for example
`server/Mawasem.Application/Features/Categories/Contracts/Responses/CategoryListResponse.cs`.
Customer types duplicate the shape locally in
`client/dashboard/src/features/customers/types.ts`.

Dashboard pages request 10 rows and reset the requested page to 1 when search or
filters change. The backend catalog/customer services default to page 1, size
20, reject non-positive pages/sizes, and cap page size at 100. When no results
exist the backend returns `totalPages: 0`; the shared pagination renders at
least one display page while preventing invalid navigation.

Catalogue dropdowns and filters reuse
`client/dashboard/src/lib/catalogue-options.ts`, whose
`CATALOGUE_OPTIONS_PAGE_SIZE` is 100. These option requests load page 1 and
preserve each endpoint's required filters.

## 9. Soft Delete and Restore

Brands, Categories, Collections, and Seasons implement soft delete:

- Normal lists pass `includeDeleted: false`; enabling the page switch sends
  `includeDeleted: true`.
- Backend queries add `!entity.IsDeleted` only when `IncludeDeleted` is false.
- `DELETE /<entities>/{id}` sets deletion metadata and returns 204; it does not
  remove the row physically.
- List responses expose camel-cased `isDeleted`.
- Active rows show Edit and Delete. Deleted rows show Restore only.
- Both delete and restore require confirmation and disable their action while
  pending.
- Successful mutations invalidate the plural list key, such as `["categories"]`.

The UI currently includes compatibility reads for legacy `IsDeleted` only in
Brands and Seasons. New code should use the backend's camel-cased `isDeleted`
contract unless a live response proves otherwise.

This is feature-specific, not a universal capability. Delivery Areas also
supports soft delete and restore. Products have their own verified controller
rules. Employees and Customers use block/unblock instead. Never add a Restore
action merely because an entity inherits deletion metadata.

Customers use `isBlocked` and block/unblock endpoints, not soft deletion.

## 10. Localisation

Dashboard i18n is initialized in
`client/dashboard/src/lib/i18n/index.ts`. English and Arabic JSON files live
under:

```text
client/dashboard/src/lib/i18n/en/*.json
client/dashboard/src/lib/i18n/ar/*.json
```

Each file is organized by feature, but keys inside the JSON are flat dotted
keys such as `categories.dialog.createTitle`. The initializer spreads all files
into one `translation` namespace for each language. Add the same key to both
languages and import/spread a new feature file in `index.ts`.

Language detection checks local storage and then the browser, falls back to
English, and supports `en` and `ar`. On initialization and language changes,
`document.documentElement.lang` and `.dir` are synchronized. The sidebar also
changes sides using `i18n.dir(...)`.

Backend content is bilingual data, not static UI text. Entities commonly expose
`nameAr`/`nameEn` (and sometimes description/full-name equivalents). Column,
action, and relation-option UI selects the Arabic field when
`i18n.resolvedLanguage === "ar"` and the English field otherwise. Never copy
backend entity content into translation JSON files: it is persisted, runtime
data that can change independently of a frontend build, whereas the JSON files
contain static interface text.

## 11. Current Known Constraints

- The storefront is a visual scaffold: its auth forms do not submit, and its
  home page is a coming-soon image.
- The dashboard home page is placeholder text.
- Brands is not fully localized; its page, columns, dialogs, form labels,
  validation messages, and actions contain hardcoded English.
- `client/dashboard/src/features/brands/api/get-brand.ts` calls
  `/brand/{id}`, while `AdminBrandsController` exposes `/api/admin/brands/{id}`.
  The correct behavior of any deployed proxy rewrite is **Needs verification**;
  do not copy the singular frontend path.
- Brand columns are domain-specific but currently live at
  `client/dashboard/src/components/entity-table/columns/brand-columns.tsx`.
  New domain columns belong inside their feature, as Categories, Collections,
  Seasons, and Customers demonstrate.
- Empty unused/legacy column files remain under
  `client/dashboard/src/components/entity-table/columns`; their existence is
  not a pattern to extend.
- Two confirmation APIs coexist:
  `EntityDeleteDialog`/`EntityRestoreDialog` and `DeleteEntityDialog`.
- Catalog details API functions and hooks exist, but no catalog details pages
  are routed. Employees uses a details dialog backed by its single-employee
  endpoint.
- `ProtectedRoute` navigates to `/change-password`, while the declared route is
  `/auth/change-password`. The intended forced-password-change URL is
  **Needs verification**.
- List-hook and mutation-hook return names are not perfectly uniform across
  features. Follow the selected reference feature rather than mixing styles.
- No frontend test script exists in either client `package.json`. Backend xUnit
  tests exist separately.

## 12. Verified Backend Architecture

The backend is a layered .NET 8 solution, but its concrete repository structure
is the contract:

- `server/Mawasem.Domain`: entities, value objects, enums, identity role and
  permission constants, and domain-level interfaces.
- `server/Mawasem.Application`: request/response records, result/error models,
  feature service interfaces, authentication options, and application-facing
  contracts. It references Domain.
- `server/Mawasem.Infrastructure`: service implementations, ASP.NET Core
  Identity integration, EF Core `MawasemDbContext`, configurations, migrations,
  seeders, storage, and persistence logic. It references Application and
  Domain.
- `server/Mawasem.API`: controllers, JWT bearer setup, cookie handling,
  permission authorization, dependency registration, and HTTP
  `ProblemDetails` mapping. It is the startup project and references the other
  backend projects.
- `server/Mawasem.Tests`: xUnit unit, workflow, authorization, authentication,
  and integration tests. It references all backend projects.

The verified management request flow is:

```text
Controller
  -> Application service interface and request/response contract
  -> Infrastructure service implementation
  -> EF Core and/or ASP.NET Core Identity
  -> SQL Server
```

Controllers do not use a separate Application use-case implementation layer.
For frontend work, verify both the controller and Application contracts, then
inspect the Infrastructure implementation for validation and business rules.

## 13. Authentication and Authorization

### Authentication flows

ASP.NET Core Identity uses `ApplicationUser`, integer keys, and SQL-backed
Identity stores. JWT bearer validation is configured in
`server/Mawasem.API/Program.cs`. The bearer handler reads the JWT access token
from the `accessToken` cookie.

Dashboard employees use `api/admin/auth`; customers use `api/auth`. Both flows
issue an access token and rotating refresh token, but the serialized login and
refresh responses contain only the `user` object: token fields are marked
`JsonIgnore`. The API writes tokens to cookies with `HttpOnly`, `Secure`,
`SameSite=None`, and `IsEssential` enabled:

- access token: shared `accessToken` cookie, path `/`;
- dashboard refresh token: separate cookie scoped to `/api/admin/auth`;
- customer refresh token: separate cookie scoped to `/api/auth`.

The two refresh-token cookies must not be treated as interchangeable. Logout,
password change/reset, blocking, and role changes revoke tokens where the
corresponding service explicitly does so. Refresh uses rotation; reuse of an
already-replaced dashboard refresh token causes remaining active tokens for
that user to be revoked.

Dashboard login requires an unblocked user with at least one dashboard role and
valid credentials. Customer login is a separate service and role flow. A
Customer does not gain dashboard authorization even if a direct permission is
accidentally assigned.

### Permission authorization

Dashboard controllers use `RequirePermissionAttribute`, which maps a registered
`SystemPermissions` value to an ASP.NET Core authorization policy. The
`PermissionAuthorizationHandler` reloads authorization state from the database;
it does not rely only on permissions embedded in an earlier JWT.

Authorization succeeds only when the user:

- is authenticated and has a valid user ID;
- is not blocked;
- does not have `MustChangePassword` set;
- has at least one dashboard role;
- has the active requested permission through a dashboard role or a direct user
  assignment.

Therefore:

```text
effectivePermissions = role permissions + directPermissions
```

The union is distinct. `effectivePermissions` is a display/authorization
summary. Only `directPermissions` is sent to the employee direct-permissions
update endpoint. A dashboard employee with `mustChangePassword: true` may use
the `[Authorize]` change-password endpoint, but permission-protected dashboard
operations are denied until the password changes. Blocked employees are also
denied, and active refresh tokens are revoked when an employee is blocked.

## 14. Employees Backend Contract

All paths below are full backend paths. Dashboard Axios calls omit
`/api/admin` because its configured base URL already includes that prefix.

| Method and full path | Permission | Request | Success |
| --- | --- | --- | --- |
| `GET /api/admin/employees` | `Employees.View` | query below | `200` paginated list |
| `GET /api/admin/employees/{employeeId}` | `Employees.View` | positive integer ID | `200` employee |
| `GET /api/admin/employees/access-options` | `Employees.View` | none | `200` assignable roles and direct permissions |
| `POST /api/admin/employees` | `Employees.Create` | create contract below | `201` employee |
| `PUT /api/admin/employees/{employeeId}` | `Employees.Edit` | profile contract below | `200` employee |
| `POST /api/admin/employees/{employeeId}/block` | `Employees.Block` | `{ "reason": "string" }` | `204` |
| `POST /api/admin/employees/{employeeId}/unblock` | `Employees.Unblock` | no body | `204` |
| `POST /api/admin/employees/{employeeId}/reset-password` | `Employees.ResetPassword` | temporary-password contract below | `204` |
| `PUT /api/admin/employees/{employeeId}/roles` | `Employees.AssignRoles` | `{ "roleNames": ["string"] }` | `200` employee |
| `PUT /api/admin/employees/{employeeId}/permissions` | `Employees.AssignPermissions` | `{ "permissionNames": ["string"] }` | `200` employee |

List query parameters are `search`, optional `isBlocked`, `pageNumber`
(default 1), and `pageSize` (default 20, maximum 100). Search covers Arabic and
English full names and email. The response uses the shared pagination fields
and each employee item contains:

```text
id, fullNameAr, fullNameEn, email,
isBlocked, blockedAt, blockedReason, mustChangePassword,
roles, directPermissions, effectivePermissions
```

The update body is:

```json
{
  "fullNameAr": "string",
  "fullNameEn": "string",
  "email": "string"
}
```

The create body adds `temporaryPassword`,
`confirmTemporaryPassword`, `roleNames`, and `permissionNames` to those three
profile fields. Password examples must use placeholders only.

`GET /api/admin/employees/access-options` is the source of truth for what the
current actor may assign:

- `SuperAdmin` is excluded from employee role choices.
- `Admin` is excluded for non-SuperAdmin actors.
- `Dashboard.Access` is excluded from direct-permission choices.
- SuperAdmin may assign any other active registered system permission.
- A non-SuperAdmin may assign only active permissions already in that actor's
  effective permission set.
- At least one dashboard role is required when role assignments are updated.

Employee role and permission management must not use
`GET /api/admin/roles/permission-options`; that endpoint has role-specific
semantics. Initialize a direct-permission dialog from `directPermissions`, not
`effectivePermissions`, and submit only the currently assignable selected
direct permissions.

SuperAdmin employee accounts cannot be managed through these assignment,
block/unblock, or password-reset operations. Users cannot perform those
sensitive operations on themselves. Non-SuperAdmins cannot manage an employee
who currently has the Admin role.

### Employee password reset

`POST /api/admin/employees/{employeeId}/reset-password` accepts:

```json
{
  "temporaryPassword": "<temporary-password>",
  "confirmTemporaryPassword": "<temporary-password>"
}
```

On success it returns `204 No Content`, sets `mustChangePassword` to `true`,
clears `LockoutEnd` and `AccessFailedCount`, and revokes all active refresh
tokens for that employee. Never log or persist a temporary password, put it in
`localStorage`, or place it in TanStack Query cache.

## 15. Roles and Permissions Backend Contract

| Method and full path | Permission | Success |
| --- | --- | --- |
| `GET /api/admin/roles` | `Roles.View` | `200` `{ items }` |
| `GET /api/admin/roles/{roleName}` | `Roles.View` | `200` role |
| `GET /api/admin/roles/permission-options` | `Roles.View` | `200` `{ items }` |
| `PUT /api/admin/roles/{roleName}/permissions` | `Roles.ManagePermissions` | `200` updated role |

The update body is `{ "permissionNames": ["string"] }`. A role response
contains `name`, `isProtected`, `canManagePermissions`, `assignedUserCount`,
and `permissionNames`.

`SuperAdmin` and `Customer` are protected; their role permissions cannot be
managed. A non-SuperAdmin cannot manage the `Admin` role. Permission options
are limited to active registered permissions the actor may assign. Unlike
employee direct-permission options, role permission-options may include
`Dashboard.Access` and mark it `isRequired: true`. The update service requires
the actor to be able to preserve that permission and always adds it to editable
dashboard roles, even if the request omits it. Do not reuse this endpoint for
employee permissions.

## 16. Delivery Areas Backend Contract

`DeliveryAreaStatus` is defined in
`server/Mawasem.Domain/Enums/DeliveryAreaStatus.cs`:

```text
Pending = 1
Confirmed = 2
Restricted = 3
```

| Method and full path | Permission | Request | Success |
| --- | --- | --- | --- |
| `GET /api/admin/delivery-areas` | `DeliveryAreas.View` | query below | `200` paginated list |
| `GET /api/admin/delivery-areas/{deliveryAreaId}` | `DeliveryAreas.View` | positive integer ID | `200` area |
| `POST /api/admin/delivery-areas` | `DeliveryAreas.Create` | create body below | `201` area |
| `PUT /api/admin/delivery-areas/{deliveryAreaId}` | `DeliveryAreas.Edit` | standard update body below | `200` area |
| `PUT /api/admin/delivery-areas/{deliveryAreaId}/status` | `DeliveryAreas.Edit` | `{ "status": 1\|2\|3 }` | `200` area |
| `DELETE /api/admin/delivery-areas/{deliveryAreaId}` | `DeliveryAreas.Delete` | none | `204` |
| `POST /api/admin/delivery-areas/{deliveryAreaId}/restore` | `DeliveryAreas.Delete` | no body | `204` |
| `GET /api/delivery-areas` | anonymous | none | `200` public active/confirmed areas |

Admin list query parameters are `search`, `status`, `isActive`,
`includeDeleted`, `pageNumber` (default 1), and `pageSize` (default 20,
maximum 100). Each list/detail item contains:

```text
id, nameAr, nameEn, status,
deliveryFee, effectiveDeliveryFee, isFreeDelivery, isActive,
activeAddressCount, isDeleted,
createdOn, createdBy, lastModifiedOn, lastModifiedBy,
deletedOn, deletedBy
```

Create:

```json
{
  "nameAr": "string",
  "nameEn": "string",
  "deliveryFee": 0,
  "isFreeDelivery": false,
  "isActive": true,
  "status": 2
}
```

`isActive` defaults to `true` and `status` defaults to `Confirmed` when omitted
by model binding. Standard update is:

```json
{
  "nameAr": "string",
  "nameEn": "string",
  "deliveryFee": 0,
  "isFreeDelivery": false,
  "isActive": true
}
```

It does not update `status`. Status changes use the dedicated `/status`
endpoint:

```json
{
  "status": 3
}
```

Verified rules:

- `isFreeDelivery: true` normalizes the stored `deliveryFee` to `0`;
  `effectiveDeliveryFee` is also `0`.
- Public results contain only areas that are not deleted, active, and
  `Confirmed`.
- An area with any active, non-deleted customer address cannot be deleted.
  The API returns `409` with code
  `delivery_areas.has_active_addresses`; management should restrict or
  deactivate it instead.
- Restore can return `409` with `delivery_areas.duplicate_name` when another
  active area now uses the deleted area's Arabic or English name.
- Delete and restore return `204`.
- There is no `DeliveryAreas.Restore` permission. The current controller
  requires `DeliveryAreas.Delete` for restore.

A dashboard Delivery Areas table must show status, active state,
free-delivery state, stored fee, effective fee, address count, and deletion
state as distinct concepts.

## 17. Collections, Seasons, and Migration State

`Collection` has a required integer `SeasonId` and a required navigation to
`Season`. EF Core configures:

```text
Collection -> Season
many Collections to one Season
foreign key Collections.SeasonId
delete behavior Restrict
```

Create and update collection requests require `seasonId`. The Infrastructure
service requires the referenced Season to exist and not be deleted; an inactive
but non-deleted Season is currently accepted. Restore is rejected if the
collection's Season has been deleted.

Migration `20260721201331_AddSeasonToCollections` adds the non-null
`Collections.SeasonId` column, its index, and the restrictive foreign key. An
error such as `Invalid column name 'SeasonId'` usually means the runtime
database has not applied that migration. Identical source code does not
guarantee identical local schema; migration history must also match.

## 18. Errors, Soft Delete, and Database Diagnostics

### Problem details

Management controllers map service failures to ASP.NET Core `ProblemDetails`.
The relevant JSON fields are:

```text
title, status, detail, code
```

`code` is written as a ProblemDetails extension. Handle known codes only.
Verified examples include `employees.invalid_permission`,
`delivery_areas.has_active_addresses`, and
`delivery_areas.duplicate_name`. Do not invent codes from UI labels.

### Soft delete and restore

For a feature that supports soft delete:

- `includeDeleted: false` excludes deleted rows; `true` includes both active
  and deleted rows unless the service documents a different filter.
- use `isDeleted` to show Delete for active rows and Restore for deleted rows;
- treat a successful `204 No Content` as success without parsing a body;
- invalidate the feature's list query after delete or restore;
- render backend deletion conflicts instead of assuming delete always works;
- keep restore conflict errors visible and let the user resolve them.

Restore support is verified for Brands, Categories, Collections, Seasons,
Delivery Areas, and feature-specific Product operations. It is not a universal
base-entity operation.

### Database migration diagnostic

When code and a local database disagree:

1. inspect the entity and EF Core configuration;
2. inspect the migration that introduces the column, index, constraint, or
   relationship;
3. inspect the target database's `__EFMigrationsHistory` table without copying
   connection values into logs or docs;
4. from `server`, apply repository migrations with:

   ```text
   dotnet ef database update --project Mawasem.Infrastructure --startup-project Mawasem.API
   ```

5. compare local migration history with the migration files and current model
   snapshot.

This command uses `Mawasem.API` configuration and may require local User
Secrets or environment configuration. Never document or paste their values.

## 19. Store Checkout Delivery Methods

The customer checkout endpoints are:

| Method and full path | Request | Success |
| --- | --- | --- |
| `POST /api/checkout/preview` | preview contract below | `200` preview |
| `POST /api/checkout/place-order` | place-order contract below | `201` new order or `200` idempotent replay |

`DeliveryMethod` is defined in
`server/Mawasem.Domain/Enums/DeliveryMethod.cs`:

```text
HomeDelivery = 1
StorePickup = 2
```

Both requests contain nullable `userAddressId`, `deliveryMethod`, and
`paymentMethod`. Place order additionally contains nullable `notes` and a
required `idempotencyKey`.

Verified rules:

- Home Delivery requires a positive `userAddressId`, verifies ownership and
  active state, requires an active confirmed delivery area, and uses that
  area's effective delivery fee.
- Store Pickup does not load an address or delivery area. Its delivery fee is
  zero and both preview address identifiers are null.
- Store Pickup orders persist `DeliveryMethod.StorePickup`, keep
  `UserAddressId` and `ShippingDeliveryAreaId` null, and leave every immutable
  shipping snapshot null.
- Unsupported delivery methods return
  `checkout.invalid_delivery_method`; missing Home Delivery addresses return
  `checkout.address_required`.
- Checkout preview includes the selected delivery method. Place-order and
  customer/admin order responses also expose the persisted delivery method.
- The existing order schema already permits nullable address relationships and
  shipping snapshots and constrains delivery methods to values `1` and `2`.
  This checkout change requires no migration.
