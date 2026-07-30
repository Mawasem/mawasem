import { Search } from "lucide-react"
import { useState, type FormEvent } from "react"
import { useNavigate } from "react-router-dom"

import { Input } from "@/components/ui/input"

export function NavbarSearch() {
  const navigate = useNavigate()
  const [searchQuery, setSearchQuery] = useState("")

  function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    const trimmedQuery = searchQuery.trim()

    if (!trimmedQuery) {
      return
    }

    navigate(`/products?search=${encodeURIComponent(trimmedQuery)}`)
  }

  return (
    <form
      onSubmit={handleSearch}
      className="relative mx-auto hidden max-w-md flex-1 md:block"
    >
      <Search className="absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" />

      <Input
        type="search"
        value={searchQuery}
        onChange={(event) => setSearchQuery(event.target.value)}
        placeholder="Search for products..."
        className="pl-9"
      />
    </form>
  )
}
