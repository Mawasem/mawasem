import { zodResolver } from "@hookform/resolvers/zod"
import { LoaderCircle } from "lucide-react"
import { useForm } from "react-hook-form"
import { Navigate, useNavigate, useSearchParams } from "react-router-dom"
import { Button } from "@/components/ui/button"
import {
  Field,
  FieldError,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { useResetCustomerPassword } from "../hooks/use-reset-customer-password"
import {
  resetPasswordSchema,
  type ResetPasswordFormValues,
} from "../schemas/password-reset-schemas"
import { getApiErrorMessage } from "@/lib/get-api-error-message"
export default function ResetPasswordPage() {
  const [params] = useSearchParams()
  const phoneNumber = params.get("phone") ?? ""
  const resetToken = params.get("token") ?? ""
  const navigate = useNavigate()
  const mutation = useResetCustomerPassword()
  const form = useForm<ResetPasswordFormValues>({
    resolver: zodResolver(resetPasswordSchema),
    defaultValues: {
      phoneNumber,
      resetToken,
      newPassword: "",
      confirmNewPassword: "",
    },
  })
  if (!phoneNumber || !resetToken)
    return <Navigate to="/auth/forgot-password" replace />
  const onSubmit = async (values: ResetPasswordFormValues) => {
    try {
      await mutation.resetPasswordAsync(values)
      navigate("/auth/login", { replace: true })
    } catch {
      //
    }
  }
  return (
    <form
      className="flex flex-col gap-6"
      onSubmit={form.handleSubmit(onSubmit)}
    >
      <FieldGroup>
        <div className="text-center">
          <h1 className="text-2xl font-bold">Choose a new password</h1>
        </div>
        <input type="hidden" {...form.register("phoneNumber")} />
        <input type="hidden" {...form.register("resetToken")} />
        <Field>
          <FieldLabel htmlFor="newPassword">New password</FieldLabel>
          <Input
            id="newPassword"
            type="password"
            autoComplete="new-password"
            {...form.register("newPassword")}
          />
          <FieldError errors={[form.formState.errors.newPassword]} />
        </Field>
        <Field>
          <FieldLabel htmlFor="confirmNewPassword">
            Confirm new password
          </FieldLabel>
          <Input
            id="confirmNewPassword"
            type="password"
            autoComplete="new-password"
            {...form.register("confirmNewPassword")}
          />
          <FieldError errors={[form.formState.errors.confirmNewPassword]} />
        </Field>
        {mutation.error ? (
          <p className="text-sm text-destructive">
            {getApiErrorMessage(
              mutation.error,
              "Could not reset the password."
            )}
          </p>
        ) : null}
        <Button disabled={mutation.isLoading}>
          {mutation.isLoading ? (
            <>
              <LoaderCircle className="animate-spin" />
              Resetting...
            </>
          ) : (
            "Reset password"
          )}
        </Button>
      </FieldGroup>
    </form>
  )
}
