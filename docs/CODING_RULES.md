# Mawasem Coding Rules

## 1. General Rules

- Frontend source is TypeScript/TSX. Preserve strict typing.
- Do not use `any` unless a real external boundary makes it unavoidable;
  document and narrow it as soon as possible. Current client source contains no
  explicit `any`.
- Keep files and components focused on one responsibility.
- Prefer composition of existing shared components over new inheritance or
  wrapper hierarchies.
- Avoid speculative abstractions; a one-off domain component stays in its
  feature.
- Inspect the matching backend controller and contracts before implementing a
  frontend request.
- Do not modify unrelated files or normalize an entire feature while making a
  narrow change.
- Preserve current architecture unless the task explicitly requires and
  justifies a broader change.

## 2. Naming Conventions

Verified frontend naming patterns:

| Kind | Pattern | Examples |
| --- | --- | --- |
| API file | kebab-case verb + singular/plural noun | `get-categories.ts`, `update-category.ts` |
| Hook file | `use-` + kebab-case operation/entity | `use-categories.ts`, `use-restore-category.ts` |
| Feature component file | kebab-case | `category-dialog.tsx`, `collection-form.tsx` |
| Routed page file | PascalCase in current dashboard | `CategoriesPage.tsx`, `BrandsPage.tsx` |
| Component export | PascalCase | `CategoryDialog`, `EntityTable` |
| Hook export | camelCase beginning `use` | `useCategoryColumns`, `useCollections` |
| API function | camelCase verb + entity | `getCategory`, `createCollection` |
| Entity/payload type | PascalCase | `Category`, `CategoryPayload` |
| Operation params | PascalCase ending `Params` | `UpdateCategoryParams` |
| Form type | PascalCase ending `FormValues` | `CollectionFormValues` |
| Schema factory | `create...FormSchema` | `createCategoryFormSchema` |
| Translation key | flat dotted feature prefix | `categories.dialog.createTitle` |
| List query key | plural feature string + params | `["categories", params]` |
| Detail query key | singular feature string + ID | `["category", id]` |

Some legacy names differ (`create-seasons.ts`, `CustomerPage.tsx`). Do not rename
them in an unrelated task.

## 3. Import Conventions

- Use `@/` for shared modules and cross-feature imports.
- Use relative imports within the same feature.
- Use `import type` for type-only imports; `verbatimModuleSyntax` is enabled.
- Keep third-party imports, shared alias imports, and local feature imports
  readable in groups. No lint rule currently enforces a specific import order.
- Do not add file extensions unless required. Existing `main.tsx` has one
  `.tsx` alias import, but most code omits extensions.
- Follow `.prettierrc`: double quotes, no semicolons, two spaces, LF, trailing
  commas where ES5 permits, print width 80, and Tailwind class sorting.
- Formatting is inconsistent in older files. Configured Prettier behavior is
  the source of truth for changed lines; do not reformat unrelated code.

## 4. Type Rules

- Store reusable entity, payload, update-param, query-param, and response types
  in the owning feature's `types.ts` or `types/` directory.
- Use `client/dashboard/src/types/pagination.ts` for reusable paginated
  responses instead of recreating the five pagination fields.
- Infer form values from Zod with `z.infer` when the schema owns the shape.
- Component prop types shared by several feature components may live in
  `components/types.ts`; small private prop types may remain beside the
  component.
- Shared component prop types live beside the shared component group, for
  example `components/entity-dialog/types.ts`.
- Use `Update<Entity>Params` to combine route ID and request data when a mutation
  accepts both.
- Do not duplicate a backend contract in several component files.
- Keep types honest about response status. A 204 operation should not promise
  an entity response.

## 5. API Rules

- Put one HTTP operation in each API file.
- Import and use `api` from `@/lib/axios`.
- Pass query parameters using Axios `params`; never concatenate them manually.
- Use dashboard-relative paths such as `/categories`, because
  `VITE_API_URL` already ends at `/api/admin`.
- Type request parameters and response data.
- Return `response.data` (or destructured `data`) for response-bearing calls.
- Await 204 operations without inventing response content.
- Do not catch Axios errors unless the API layer is adding meaningful,
  reusable boundary behavior. Normal errors propagate to TanStack Query.
- Do not include hooks, cache invalidation, translation, or UI decisions in API
  files.
- Confirm route, HTTP method, parameters, status, and contract from the backend
  controller/Application records before coding.
- For details dialogs, use the single-entity endpoint instead of assuming list
  row data is complete.
- Gate dialog-only queries with `enabled` while the dialog is closed.
- Treat `204 No Content` as success without parsing response data.

## 6. Hook Rules

- Keep one exported Query or mutation hook per file.
- Query hooks call feature API functions; they do not call Axios directly.
- Include every list parameter in the query key by using the params object.
- Use plural keys for lists and singular keys plus ID for details.
- Gate numeric details queries with `enabled: !!id`.
- Expose data, error, and pending state explicitly. List features normally
  rename `isPending` to `isLoading`.
- Give mutation functions domain-specific names when returning an explicit
  object; keep naming consistent within the feature.
- Invalidate the plural list-key prefix after successful create, update,
  delete, restore, block, or unblock.
- Do not put JSX, dialog state, translated labels, or column definitions in a
  hook.
- Do not add a duplicate hook for an operation that already exists.

## 7. Component Rules

- Reuse `EntityManagementPage` for the standard searchable CRUD list with
  include-deleted behavior.
- Reuse `EntityTable`, `EntityToolbar`, and `EntityPagination`; do not create
  feature-specific copies.
- Reuse `EntityDialog` and `EntityDialogFooter` for create/edit forms.
- Reuse the existing confirmation components; do not introduce a third
  confirmation API.
- Keep domain logic, hooks, columns, forms, and actions out of global entity
  components.
- Keep domain columns in the feature even though the legacy Brand columns are
  misplaced.
- Column definitions render values/actions only; they never call APIs.
- Action dropdowns open dialogs; destructive operations require confirmation.
- Do not create a generic abstraction for a one-off component.

## 8. Form Rules

- Use React Hook Form and Zod through `zodResolver`.
- Keep the schema, inferred form values, and create defaults in the feature
  schema file.
- Build validation messages with the current language's `t` function. Do not
  copy the hardcoded Brands validation pattern.
- Translate labels, placeholders, hints, action text, loading text, and
  validation messages.
- Initialize create defaults and edit values deliberately.
- Call `form.reset(...)` when the edited entity or mode changes.
- Give the form a stable ID and pass exactly that ID to `EntityDialogFooter`.
- Keep submit async and allow the dialog to await it.
- Convert relation IDs from DOM strings to numbers before validation/payload
  construction. The Collection `seasonId` pattern is the reference.
- Use a sentinel such as `0` only when the schema explicitly rejects it.
- Disable submit/cancel while the relevant mutation is pending.
- Display the mutation error in the form/dialog and keep the dialog open on
  failure.
- Relation dropdowns must handle loading, backend page limits, and localized
  option labels.

## 9. UI Rules

- Compose primitives from `client/dashboard/src/components/ui`.
- Reuse project wrappers before adding raw Radix/shadcn structure.
- Match existing spacing, typography, rounded shapes, color tokens, and
  destructive variants.
- Use Tailwind logical direction utilities (`ms`, `me`) where direction matters.
- Preserve document and sidebar RTL/LTR behavior.
- Use Lucide icons consistently with current dashboard feature code. Do not mix
  icon systems within one interaction without an existing reason.
- Pass translated labels into shared components instead of relying on their
  English fallback strings.
- Preserve loading, error, empty, and disabled states.
- Do not introduce a new visual pattern without a requirement.

## 10. Localisation Rules

- No new hardcoded user-facing strings.
- Add every static UI key to both `lib/i18n/en/<feature>.json` and
  `lib/i18n/ar/<feature>.json`.
- Preserve flat dotted keys and a feature prefix.
- Import and spread a new translation file in `lib/i18n/index.ts`.
- Keep backend entity data out of translation JSON.
- For bilingual response fields, select `nameAr`/`descriptionAr`/`fullNameAr`
  only when the resolved language is Arabic; otherwise select the English
  counterpart.
- Add `i18n.resolvedLanguage` to memo dependencies when it affects rendered
  columns/options.
- Check both RTL and LTR layouts.

## 11. Delete and Restore Rules

- Active entity: expose Edit and Delete.
- Deleted entity: expose Restore only.
- Use camel-cased `isDeleted` from the current backend JSON contract.
- Require confirmation before delete or restore.
- Pass a localized entity name when it improves confirmation clarity.
- Disable the dropdown/confirmation action while its mutation is pending.
- Close the confirmation only after `mutateAsync` succeeds.
- On failure, keep it open and display the mutation error.
- Invalidate the plural list query after success.
- Do not physically delete records from frontend assumptions; the verified
  catalog and Delivery Areas delete endpoints are soft-delete operations.
- Do not apply this model to Customers, which uses block/unblock and
  `isBlocked`.
- Do not assume every `BaseAuditableEntity` has a restore endpoint. Verify
  restore per controller.
- Surface backend deletion conflicts and restoration name conflicts in the
  confirmation dialog. Do not optimistically remove the row on a failed
  operation.
- Delivery Areas restore currently requires `DeliveryAreas.Delete`; do not
  invent `DeliveryAreas.Restore`.

## 12. Employees, Roles, and Delivery Areas Rules

### Employees

- Use `GET /employees/access-options` for employee role and direct-permission
  choices. Do not use `/roles/permission-options`.
- Initialize permission editing from `directPermissions`.
- Treat `effectivePermissions` as a display-only union of role and direct
  permissions.
- Submit only direct permission names to `/employees/{id}/permissions`.
- Required role permissions such as `Dashboard.Access` are not automatically
  directly assignable to an employee.
- Display compact role/permission counts in tables. Put large permission sets
  in grouped, searchable, scrollable dialogs.
- Never log, persist in `localStorage`, or cache temporary passwords. Password
  examples may contain placeholders only.

### Roles

- Use `GET /roles/permission-options` only for role permission management.
- Honor response fields `isProtected` and `canManagePermissions`.
- Keep required `Dashboard.Access` selected for editable dashboard roles.
- Do not offer permission management for protected `SuperAdmin` or `Customer`.

### Delivery Areas

- Model `DeliveryAreaStatus` exactly: Pending `1`, Confirmed `2`, Restricted
  `3`.
- Standard update does not contain status; call the dedicated `/status`
  endpoint.
- Display status, active state, free-delivery state, stored fee, effective fee,
  active-address count, and deletion state distinctly.
- When `isFreeDelivery` is true, expect stored and effective fees to be zero.
- Handle `delivery_areas.has_active_addresses` as a conflict and guide
  management toward restricting or deactivating the area.
- Handle duplicate-name conflicts during restore.

## 13. Database and Migration Rules

- Source and local database schema can differ when migrations are unapplied.
- For a missing-column error, inspect the entity, EF configuration, matching
  migration, model snapshot, and target `__EFMigrationsHistory` first.
- `Collections.SeasonId` is introduced by
  `20260721201331_AddSeasonToCollections`.
- From `server`, the verified repository command shape is:

  ```text
  dotnet ef database update --project Mawasem.Infrastructure --startup-project Mawasem.API
  ```

- Do not place connection strings or secret configuration values in commands,
  docs, logs, examples, or commits.

## 14. Prohibited Changes

- No architecture replacement for an ordinary feature task.
- No rewriting shared components without a demonstrated requirement.
- No duplicate tables, pagination, toolbars, forms, hooks, API functions, or
  confirmation UI.
- No moving feature-specific behavior into global components.
- No secret values in code, logs, docs, examples, or commits.
- No unrelated feature changes or broad formatting churn.
- No changing backend request/response contracts from frontend code.
- No assuming that a translation file, sidebar label, backend controller, or
  unused hook means a routed UI is implemented.
- No copying known inconsistencies from Brands or the Collections page-size
  mismatch into new features.
