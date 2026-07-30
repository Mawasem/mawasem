import { Card, CardContent } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"

export function ProductCardSkeleton() {
  return (
    <Card className="overflow-hidden py-0">
      <Skeleton className="aspect-square w-full rounded-none" />
      <CardContent className="space-y-3 p-4">
        <Skeleton className="h-3 w-24" />
        <Skeleton className="h-5 w-full" />
        <Skeleton className="h-5 w-2/3" />
        <Skeleton className="h-9 w-full" />
      </CardContent>
    </Card>
  )
}
