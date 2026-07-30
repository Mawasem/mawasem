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
import { useVerifyCustomerPasswordResetCode } from "../hooks/use-verify-customer-password-reset-code"
import {
  verifyResetCodeSchema,
  type VerifyResetCodeFormValues,
} from "../schemas/password-reset-schemas"
import { getApiErrorMessage } from "@/lib/get-api-error-message"
export default function VerifyResetCodePage() {
  const [params] = useSearchParams()
  const phoneNumber = params.get("phone") ?? ""
  const navigate = useNavigate()
  const mutation = useVerifyCustomerPasswordResetCode()
  const form = useForm<VerifyResetCodeFormValues>({
    resolver: zodResolver(verifyResetCodeSchema),
    defaultValues: { phoneNumber, code: "" },
  })
  if (!phoneNumber) return <Navigate to="/auth/forgot-password" replace />
  const onSubmit = async (values: VerifyResetCodeFormValues) => {
    try {
      const result = await mutation.verifyCodeAsync(values)
      navigate(
        `/auth/reset-password?phone=${encodeURIComponent(phoneNumber)}&token=${encodeURIComponent(result.resetToken)}`
      )
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
          <h1 className="text-2xl font-bold">Verify code</h1>
          <p className="text-sm text-muted-foreground">
            Enter the 6-digit code sent to your mobile.
          </p>
        </div>
        <input type="hidden" {...form.register("phoneNumber")} />
        <Field>
          <FieldLabel htmlFor="code">Verification code</FieldLabel>
          <Input
            id="code"
            inputMode="numeric"
            maxLength={6}
            {...form.register("code")}
          />
          <FieldError errors={[form.formState.errors.code]} />
        </Field>
        {mutation.error ? (
          <p className="text-sm text-destructive">
            {getApiErrorMessage(
              mutation.error,
              "The code is invalid or expired."
            )}
          </p>
        ) : null}
        <Button disabled={mutation.isLoading}>
          {mutation.isLoading ? (
            <>
              <LoaderCircle className="animate-spin" />
              Verifying...
            </>
          ) : (
            "Verify code"
          )}
        </Button>
      </FieldGroup>
    </form>
  )
}
