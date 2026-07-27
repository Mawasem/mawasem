import { useQuery } from "@tanstack/react-query";
import { getProductImages } from "../api/get-product-images";
export function useProductImages(productId: number, enabled = true) { const { data: productImagesData, isPending: isLoading, error } = useQuery({ queryKey: ["product-images", productId], queryFn: () => getProductImages(productId), enabled: enabled && productId > 0 }); return { productImagesData, isLoading, error }; }
