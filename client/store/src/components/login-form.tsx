import { zodResolver } from "@hookform/resolvers/zod"
import { LoaderCircle } from "lucide-react"
import { useForm } from "react-hook-form"
import { Link, Navigate, useLocation, useNavigate } from "react-router-dom"
import { Button } from "@/components/ui/button"
import {
  Field,
  FieldDescription,
  FieldError,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { useLoginCustomer } from "@/features/auth/hooks/use-login-customer"
import {
  loginSchema,
  type LoginFormValues,
} from "@/features/auth/schemas/login-schema"
import { useCustomerAuthStore } from "@/features/auth/store/use-customer-auth-store"
import { getApiErrorMessage } from "@/lib/get-api-error-message"
import { cn } from "@/lib/utils"

export function LoginForm({
  className,
  ...props
}: Omit<React.ComponentProps<"form">, "onSubmit">) {
  const navigate = useNavigate()
  const location = useLocation()
  const isAuthenticated = useCustomerAuthStore(
    (state) => state.status === "authenticated"
  )
  const mutation = useLoginCustomer()
  const form = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { phoneNumber: "", password: "" },
  })

  if (isAuthenticated) return <Navigate to="/" replace />

  const onSubmit = async (values: LoginFormValues) => {
    try {
      await mutation.loginAsync(values)
      const destination =
        (location.state as { from?: string } | null)?.from ?? "/"
      navigate(destination, { replace: true })
    } catch {
      /* rendered below */
    }
  }

  return (
    <form
      className={cn("flex flex-col gap-6", className)}
      onSubmit={form.handleSubmit(onSubmit)}
      noValidate
      {...props}
    >
      <FieldGroup>
        <div className="flex flex-col items-center gap-1 text-center">
          <h1 className="text-2xl font-bold">Login to your account</h1>
          <p className="text-sm text-balance text-muted-foreground">
            Use your Mawasem customer mobile number.
          </p>
        </div>
        <Field data-invalid={Boolean(form.formState.errors.phoneNumber)}>
          <FieldLabel htmlFor="phoneNumber">Mobile number</FieldLabel>
          <Input
            id="phoneNumber"
            type="tel"
            autoComplete="tel"
            placeholder="01012345678"
            disabled={mutation.isLoading}
            {...form.register("phoneNumber")}
          />
          <FieldError errors={[form.formState.errors.phoneNumber]} />
        </Field>
        <Field data-invalid={Boolean(form.formState.errors.password)}>
          <div className="flex items-center">
            <FieldLabel htmlFor="password">Password</FieldLabel>
            <Link
              to="/auth/forgot-password"
              className="ms-auto text-sm underline-offset-4 hover:underline"
            >
              Forgot your password?
            </Link>
          </div>
          <Input
            id="password"
            type="password"
            autoComplete="current-password"
            disabled={mutation.isLoading}
            {...form.register("password")}
          />
          <FieldError errors={[form.formState.errors.password]} />
        </Field>
        {mutation.error ? (
          <p role="alert" className="text-sm text-destructive">
            {getApiErrorMessage(
              mutation.error,
              "Invalid mobile number or password."
            )}
          </p>
        ) : null}
        <Button type="submit" disabled={mutation.isLoading}>
          {mutation.isLoading ? (
            <>
              <LoaderCircle className="animate-spin" />
              Signing in...
            </>
          ) : (
            "Sign in"
          )}
        </Button>
        <FieldDescription className="text-center">
          Don&apos;t have an account?{" "}
          <Link
            to="/auth/signup"
            className="font-medium underline underline-offset-4"
          >
            Sign up
          </Link>
        </FieldDescription>
      </FieldGroup>
    </form>
  )
}
