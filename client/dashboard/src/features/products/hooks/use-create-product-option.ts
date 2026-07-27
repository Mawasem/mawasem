import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createProductOption } from "../api/create-product-option";
export function useCreateProductOption() { const queryClient = useQueryClient(); const { mutate: createProductOptionMutation, mutateAsync: createProductOptionAsync, isPending: isLoading, error } = useMutation({ mutationFn: createProductOption, onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["product-options"] }); } }); return { createProductOption: createProductOptionMutation, createProductOptionAsync, isLoading, error }; }
