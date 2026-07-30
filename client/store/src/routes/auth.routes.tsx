import type { RouteObject } from "react-router-dom"
import AuthLayout from "@/layouts/AuthLayout"
import LoginPage from "@/features/auth/pages/LoginPage"
import SignupPage from "@/features/auth/pages/SignupPage"
import ForgotPasswordPage from "@/features/auth/pages/ForgotPasswordPage"
import VerifyResetCodePage from "@/features/auth/pages/VerifyResetCodePage"
import ResetPasswordPage from "@/features/auth/pages/ResetPasswordPage"

export const authRoutes: RouteObject = {
  path: "/auth",
  element: <AuthLayout />,
  children: [
    { path: "login", element: <LoginPage /> },
    { path: "signup", element: <SignupPage /> },
    { path: "forgot-password", element: <ForgotPasswordPage /> },
    { path: "verify-reset-code", element: <VerifyResetCodePage /> },
    { path: "reset-password", element: <ResetPasswordPage /> },
  ],
}
