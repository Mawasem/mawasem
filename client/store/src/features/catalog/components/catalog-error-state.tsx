import { AlertCircle } from "lucide-react"

import { Button } from "@/components/ui/button"
import { getApiErrorMessage } from "@/lib/get-api-error-message"

interface CatalogErrorStateProps {
  error: unknown
}

export function CatalogErrorState({ error }: CatalogErrorStateProps) {
  return (
    <div className="grid min-h-80 place-items-center rounded-xl border border-destructive/30 p-8 text-center">
      <div className="space-y-4">
        <AlertCircle className="mx-auto size-10 text-destructive" />
        <div>
          <h2 className="font-semibold">Could not load products</h2>
          <p className="mt-1 text-sm text-muted-foreground">
            {getApiErrorMessage(error, "Please try again.")}
          </p>
        </div>
        <Button variant="outline" onClick={() => window.location.reload()}>
          Try again
        </Button>
      </div>
    </div>
  )
}
