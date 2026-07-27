import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateProductOption } from "../api/update-product-option";
export function useUpdateProductOption() { const queryClient = useQueryClient(); const { mutate: updateProductOptionMutation, mutateAsync: updateProductOptionAsync, isPending: isLoading, error } = useMutation({ mutationFn: updateProductOption, onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["product-options"] }); } }); return { updateProductOption: updateProductOptionMutation, updateProductOptionAsync, isLoading, error }; }
