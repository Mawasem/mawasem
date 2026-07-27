import { useQuery } from "@tanstack/react-query";
import { getProductVariants } from "../api/get-product-variants";
export function useProductVariants(productId: number, enabled = true) { const { data: productVariantsData, isPending: isLoading, error } = useQuery({ queryKey: ["product-variants", productId], queryFn: () => getProductVariants(productId), enabled: enabled && productId > 0 }); return { productVariantsData, isLoading, error }; }
