export function resolveMediaUrl(path: string | null | undefined) {
  if (!path) return null
  if (/^https?:\/\//i.test(path)) return path

  const apiUrl = import.meta.env.VITE_API_URL as string | undefined
  if (!apiUrl) return path

  try {
    const origin = new URL(apiUrl).origin
    return new URL(path, origin).toString()
  } catch {
    return path
  }
}
