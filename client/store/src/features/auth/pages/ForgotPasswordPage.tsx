import { zodResolver } from "@hookform/resolvers/zod"
import { LoaderCircle } from "lucide-react"
import { useForm } from "react-hook-form"
import { Link, useNavigate } from "react-router-dom"

import { Button } from "@/components/ui/button"
import {
  Field,
  FieldDescription,
  FieldError,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { getApiErrorMessage } from "@/lib/get-api-error-message"

import { useForgotCustomerPassword } from "../hooks/use-forgot-customer-password"
import {
  forgotPasswordSchema,
  type ForgotPasswordFormValues,
} from "../schemas/password-reset-schemas"

export default function ForgotPasswordPage() {
  const navigate = useNavigate()
  const forgotPasswordMutation = useForgotCustomerPassword()

  const form = useForm<ForgotPasswordFormValues>({
    resolver: zodResolver(forgotPasswordSchema),
    defaultValues: {
      phoneNumber: "",
    },
  })

  const onSubmit = async (values: ForgotPasswordFormValues) => {
    try {
      await forgotPasswordMutation.forgotPasswordAsync(values)

      navigate(
        `/auth/verify-reset-code?phone=${encodeURIComponent(
          values.phoneNumber
        )}`
      )
    } catch {
      // Mutation error is displayed below.
    }
  }

  return (
    <form
      className="flex flex-col gap-6"
      onSubmit={form.handleSubmit(onSubmit)}
    >
      <FieldGroup>
        <div className="text-center">
          <h1 className="text-2xl font-bold">Reset your password</h1>

          <p className="text-sm text-muted-foreground">
            We will send a verification code if the account exists.
          </p>
        </div>

        <Field>
          <FieldLabel htmlFor="phoneNumber">Mobile number</FieldLabel>

          <Input
            id="phoneNumber"
            type="tel"
            autoComplete="tel"
            {...form.register("phoneNumber")}
          />

          <FieldError errors={[form.formState.errors.phoneNumber]} />
        </Field>

        {forgotPasswordMutation.error ? (
          <p className="text-sm text-destructive">
            {getApiErrorMessage(
              forgotPasswordMutation.error,
              "Could not submit the request."
            )}
          </p>
        ) : null}

        <Button type="submit" disabled={forgotPasswordMutation.isLoading}>
          {forgotPasswordMutation.isLoading ? (
            <>
              <LoaderCircle className="animate-spin" />
              Sending...
            </>
          ) : (
            "Send code"
          )}
        </Button>

        <FieldDescription className="text-center">
          <Link to="/auth/login">Back to login</Link>
        </FieldDescription>
      </FieldGroup>
    </form>
  )
}
