export interface NavbarAccountProps {
  isAuthenticated: boolean
  customerName: string
  customerInitials: string
  phoneNumber?: string
  isLoggingOut: boolean
  onLogout: () => Promise<void>
}
