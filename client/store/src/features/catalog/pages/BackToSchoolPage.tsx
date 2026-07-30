import { CatalogPageShell } from "../components/catalog-page-shell"

const seasonId = Number(import.meta.env.VITE_BACK_TO_SCHOOL_SEASON_ID)

export default function BackToSchoolPage() {
  if (!Number.isInteger(seasonId) || seasonId <= 0) {
    return (
      <div className="rounded-xl border border-destructive/30 p-6">
        <h1 className="font-semibold">Back to School is not configured</h1>
        <p className="mt-2 text-sm text-muted-foreground">
          Add VITE_BACK_TO_SCHOOL_SEASON_ID to the Store environment file.
        </p>
      </div>
    )
  }

  return (
    <CatalogPageShell
      config={{
        seasonId,
        title: "Back to School",
        description:
          "Shop school essentials, stationery, books, bags, and everything needed for a successful school year.",
      }}
    />
  )
}
