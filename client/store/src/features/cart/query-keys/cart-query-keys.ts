export const cartQueryKeys = {
  all: ["cart"] as const,
  current: (identity: "customer" | string) =>
    [...cartQueryKeys.all, "current", identity] as const,
}
