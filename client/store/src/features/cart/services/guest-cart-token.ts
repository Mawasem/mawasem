const GUEST_CART_TOKEN_KEY = "mawasem.guest-cart-token"

export function getGuestCartToken() {
  return localStorage.getItem(GUEST_CART_TOKEN_KEY)
}

export function setGuestCartToken(token: string) {
  localStorage.setItem(GUEST_CART_TOKEN_KEY, token)
}

export function clearGuestCartToken() {
  localStorage.removeItem(GUEST_CART_TOKEN_KEY)
}
