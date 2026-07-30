import { Search } from "lucide-react"
import { useState, type FormEvent } from "react"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"

interface CatalogSearchProps {
  value: string
  onSearch: (value: string) => void
}

export function CatalogSearch({ value, onSearch }: CatalogSearchProps) {
  const [search, setSearch] = useState(value)

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    onSearch(search.trim())
  }

  return (
    <form onSubmit={handleSubmit} className="flex w-full gap-2">
      <div className="relative flex-1">
        <Search className="absolute start-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          type="search"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
          placeholder="Search products..."
          className="ps-9"
        />
      </div>
      <Button type="submit">Search</Button>
    </form>
  )
}
