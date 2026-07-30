import { zodResolver } from "@hookform/resolvers/zod"
import { LoaderCircle } from "lucide-react"
import { Controller, useForm } from "react-hook-form"
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { useRegisterCustomer } from "@/features/auth/hooks/use-register-customer"
import {
  signupSchema,
  type SignupFormValues,
} from "@/features/auth/schemas/signup-schema"
import {
  customerGender,
  customerReferralSource,
  type RegisterCustomerRequest,
} from "@/features/auth/types"
import { getApiErrorMessage } from "@/lib/get-api-error-message"
import { cn } from "@/lib/utils"

export function SignupForm({
  className,
  ...props
}: Omit<React.ComponentProps<"form">, "onSubmit">) {
  const navigate = useNavigate()
  const registerCustomerMutation = useRegisterCustomer()

  const form = useForm<SignupFormValues>({
    resolver: zodResolver(signupSchema),
    defaultValues: {
      fullNameAr: "",
      fullNameEn: "",
      phoneNumber: "",
      birthDate: "",
      gender: customerGender.male,
      referralSource: customerReferralSource.google,
      password: "",
      confirmPassword: "",
    },
  })

  const onSubmit = async (values: SignupFormValues) => {
    const request: RegisterCustomerRequest = {
      ...values,
      birthDate: values.birthDate || null,
    }

    try {
      await registerCustomerMutation.registerCustomerAsync(request)
      navigate("/", { replace: true })
    } catch {
      // The mutation error is rendered below while the user remains on the form.
    }
  }

  const errorMessage = registerCustomerMutation.error
    ? getApiErrorMessage(
        registerCustomerMutation.error,
        "We could not create your account. Please check your details and try again."
      )
    : null

  return (
    <form
      className={cn("flex flex-col gap-6", className)}
      onSubmit={form.handleSubmit(onSubmit)}
      noValidate
      {...props}
    >
      <FieldGroup>
        <div className="flex flex-col items-center gap-1 text-center">
          <h1 className="text-2xl font-bold">Create your account</h1>
          <p className="text-sm text-balance text-muted-foreground">
            Create your Mawasem customer account to start shopping.
          </p>
        </div>

        <div className="grid gap-5 sm:grid-cols-2">
          <Field data-invalid={Boolean(form.formState.errors.fullNameAr)}>
            <FieldLabel htmlFor="fullNameAr">Full name in Arabic</FieldLabel>
            <Input
              id="fullNameAr"
              autoComplete="name"
              dir="rtl"
              placeholder="عبدالله جمال"
              className="bg-background"
              aria-invalid={Boolean(form.formState.errors.fullNameAr)}
              disabled={registerCustomerMutation.isLoading}
              {...form.register("fullNameAr")}
            />
            <FieldError errors={[form.formState.errors.fullNameAr]} />
          </Field>

          <Field data-invalid={Boolean(form.formState.errors.fullNameEn)}>
            <FieldLabel htmlFor="fullNameEn">Full name in English</FieldLabel>
            <Input
              id="fullNameEn"
              autoComplete="name"
              placeholder="Abdallah Gamal"
              className="bg-background"
              aria-invalid={Boolean(form.formState.errors.fullNameEn)}
              disabled={registerCustomerMutation.isLoading}
              {...form.register("fullNameEn")}
            />
            <FieldError errors={[form.formState.errors.fullNameEn]} />
          </Field>
        </div>

        <Field data-invalid={Boolean(form.formState.errors.phoneNumber)}>
          <FieldLabel htmlFor="phoneNumber">Mobile number</FieldLabel>
          <Input
            id="phoneNumber"
            type="tel"
            inputMode="tel"
            autoComplete="tel"
            placeholder="01012345678"
            className="bg-background"
            aria-invalid={Boolean(form.formState.errors.phoneNumber)}
            disabled={registerCustomerMutation.isLoading}
            {...form.register("phoneNumber")}
          />
          <FieldDescription>
            Use an Egyptian number beginning with 010, 011, 012, or 015.
          </FieldDescription>
          <FieldError errors={[form.formState.errors.phoneNumber]} />
        </Field>

        <div className="grid gap-5 sm:grid-cols-2">
          <Field data-invalid={Boolean(form.formState.errors.birthDate)}>
            <FieldLabel htmlFor="birthDate">Birth date</FieldLabel>
            <Input
              id="birthDate"
              type="date"
              className="bg-background"
              aria-invalid={Boolean(form.formState.errors.birthDate)}
              disabled={registerCustomerMutation.isLoading}
              {...form.register("birthDate")}
            />
            <FieldDescription>Optional</FieldDescription>
            <FieldError errors={[form.formState.errors.birthDate]} />
          </Field>

          <Controller
            control={form.control}
            name="gender"
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel htmlFor="gender">Gender</FieldLabel>
                <Select
                  value={String(field.value)}
                  onValueChange={(value) => field.onChange(Number(value))}
                  disabled={registerCustomerMutation.isLoading}
                >
                  <SelectTrigger id="gender" aria-invalid={fieldState.invalid}>
                    <SelectValue placeholder="Select gender" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value={String(customerGender.male)}>
                      Male
                    </SelectItem>
                    <SelectItem value={String(customerGender.female)}>
                      Female
                    </SelectItem>
                  </SelectContent>
                </Select>
                <FieldError errors={[fieldState.error]} />
              </Field>
            )}
          />
        </div>

        <Controller
          control={form.control}
          name="referralSource"
          render={({ field, fieldState }) => (
            <Field data-invalid={fieldState.invalid}>
              <FieldLabel htmlFor="referralSource">
                How did you hear about us?
              </FieldLabel>
              <Select
                value={String(field.value)}
                onValueChange={(value) => field.onChange(Number(value))}
                disabled={registerCustomerMutation.isLoading}
              >
                <SelectTrigger
                  id="referralSource"
                  aria-invalid={fieldState.invalid}
                >
                  <SelectValue placeholder="Select a source" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={String(customerReferralSource.facebook)}>
                    Facebook
                  </SelectItem>
                  <SelectItem value={String(customerReferralSource.instagram)}>
                    Instagram
                  </SelectItem>
                  <SelectItem value={String(customerReferralSource.tiktok)}>
                    TikTok
                  </SelectItem>
                  <SelectItem value={String(customerReferralSource.google)}>
                    Google
                  </SelectItem>
                  <SelectItem value={String(customerReferralSource.friend)}>
                    Friend
                  </SelectItem>
                  <SelectItem value={String(customerReferralSource.other)}>
                    Other
                  </SelectItem>
                </SelectContent>
              </Select>
              <FieldError errors={[fieldState.error]} />
            </Field>
          )}
        />

        <Field data-invalid={Boolean(form.formState.errors.password)}>
          <FieldLabel htmlFor="password">Password</FieldLabel>
          <Input
            id="password"
            type="password"
            autoComplete="new-password"
            className="bg-background"
            aria-invalid={Boolean(form.formState.errors.password)}
            disabled={registerCustomerMutation.isLoading}
            {...form.register("password")}
          />
          <FieldDescription>
            At least 8 characters with uppercase, lowercase, number, and symbol.
          </FieldDescription>
          <FieldError errors={[form.formState.errors.password]} />
        </Field>

        <Field data-invalid={Boolean(form.formState.errors.confirmPassword)}>
          <FieldLabel htmlFor="confirmPassword">Confirm password</FieldLabel>
          <Input
            id="confirmPassword"
            type="password"
            autoComplete="new-password"
            className="bg-background"
            aria-invalid={Boolean(form.formState.errors.confirmPassword)}
            disabled={registerCustomerMutation.isLoading}
            {...form.register("confirmPassword")}
          />
          <FieldError errors={[form.formState.errors.confirmPassword]} />
        </Field>

        {errorMessage ? (
          <p role="alert" className="text-sm text-destructive">
            {errorMessage}
          </p>
        ) : null}

        <Field>
          <Button type="submit" disabled={registerCustomerMutation.isLoading}>
            {registerCustomerMutation.isLoading ? (
              <>
                <LoaderCircle className="animate-spin" />
                Creating account...
              </>
            ) : (
              "Create account"
            )}
          </Button>
        </Field>

        <FieldDescription className="px-6 text-center">
          Already have an account?{" "}
          <Link to="/auth/login" className="font-medium">
            Sign in
          </Link>
        </FieldDescription>
      </FieldGroup>
    </form>
  )
}
