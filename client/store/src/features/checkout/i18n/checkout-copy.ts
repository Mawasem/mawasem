const checkoutCopy = {
  en: {
    page: {
      title: "Checkout",
      description: "Choose how to receive your order and confirm the details.",
    },
    deliveryMethod: {
      title: "Select delivery method",
      label: "Delivery method",
      selected: "Selected",
      homeDelivery: "Home Delivery",
      homeDeliveryDescription: "Delivered to the selected address",
      homeDeliveryFee: "Delivery fee may apply",
      storePickup: "Store Pickup",
      storePickupDescription: "Pick up from the store",
      storePickupFee: "No delivery fee",
    },
    address: {
      title: "Delivery address",
      selectedAddress: "Selected address",
      required: "Select or add a valid address to continue with home delivery.",
      empty: "Add an address to continue.",
      defaultBadge: "Default",
      select: "Select address",
      delivery: "Delivery",
      add: "Add address",
      addTitle: "Add delivery address",
      addDescription:
        "This address will be used to calculate delivery and place your order.",
      fields: {
        label: "Label",
        city: "City",
        area: "Area",
        recipientName: "Recipient name",
        recipientPhone: "Recipient phone",
        building: "Building",
        floor: "Floor",
        apartment: "Apartment",
        detailedAddress: "Detailed address",
        landmark: "Landmark",
        deliveryArea: "Delivery area",
        selectDeliveryArea: "Select delivery area",
        setDefault: "Set as default address",
      },
      defaultLabel: "Home",
      save: "Save address",
      saving: "Saving...",
      createFailed: "Could not create the address.",
      validation: {
        labelRequired: "Address label is required.",
        cityRequired: "City is required.",
        areaRequired: "Area name is required.",
        detailedAddressRequired: "Detailed address is required.",
        recipientNameRequired: "Recipient name is required.",
        recipientPhoneInvalid: "Enter a valid Egyptian mobile number.",
        deliveryAreaRequired: "Select a delivery area.",
      },
    },
    payment: {
      title: "Payment and notes",
      method: "Payment method",
      cashOnDelivery: "Cash on delivery",
      onlineUnavailable: "Online payment is not supported yet.",
      notes: "Order notes",
      notesPlaceholder: "Optional delivery instructions",
    },
    summary: {
      title: "Order summary",
      subtotal: "Subtotal",
      discount: "Discount",
      delivery: "Delivery",
      total: "Total",
      free: "Free",
      pickup: "Store Pickup",
      pickupDescription: "Your order will be ready for pickup at the store.",
    },
    actions: {
      placeOrder: "Place order",
      placingOrder: "Placing order...",
    },
    errors: {
      addressesFailed: "Could not load addresses.",
      cartFailed: "Could not load your cart.",
      previewFailed: "Could not calculate checkout.",
      placeOrderFailed: "Could not place your order.",
    },
  },
  ar: {
    page: {
      title: "إتمام الطلب",
      description: "اختر طريقة استلام طلبك وراجع التفاصيل.",
    },
    deliveryMethod: {
      title: "اختر طريقة الاستلام",
      label: "طريقة الاستلام",
      selected: "محدد",
      homeDelivery: "توصيل للمنزل",
      homeDeliveryDescription: "التوصيل إلى العنوان المحدد",
      homeDeliveryFee: "قد تُطبق رسوم توصيل",
      storePickup: "استلام من المتجر",
      storePickupDescription: "استلم طلبك من المتجر",
      storePickupFee: "بدون رسوم توصيل",
    },
    address: {
      title: "عنوان التوصيل",
      selectedAddress: "العنوان المحدد",
      required: "اختر أو أضف عنوانًا صالحًا للمتابعة بالتوصيل للمنزل.",
      empty: "أضف عنوانًا للمتابعة.",
      defaultBadge: "الافتراضي",
      select: "اختر العنوان",
      delivery: "التوصيل",
      add: "إضافة عنوان",
      addTitle: "إضافة عنوان توصيل",
      addDescription: "سيُستخدم هذا العنوان لحساب التوصيل وإتمام طلبك.",
      fields: {
        label: "اسم العنوان",
        city: "المدينة",
        area: "المنطقة",
        recipientName: "اسم المستلم",
        recipientPhone: "هاتف المستلم",
        building: "المبنى",
        floor: "الدور",
        apartment: "الشقة",
        detailedAddress: "العنوان بالتفصيل",
        landmark: "علامة مميزة",
        deliveryArea: "منطقة التوصيل",
        selectDeliveryArea: "اختر منطقة التوصيل",
        setDefault: "تعيين كعنوان افتراضي",
      },
      defaultLabel: "المنزل",
      save: "حفظ العنوان",
      saving: "جارٍ الحفظ...",
      createFailed: "تعذر إنشاء العنوان.",
      validation: {
        labelRequired: "اسم العنوان مطلوب.",
        cityRequired: "المدينة مطلوبة.",
        areaRequired: "اسم المنطقة مطلوب.",
        detailedAddressRequired: "العنوان التفصيلي مطلوب.",
        recipientNameRequired: "اسم المستلم مطلوب.",
        recipientPhoneInvalid: "أدخل رقم هاتف مصريًا صالحًا.",
        deliveryAreaRequired: "اختر منطقة توصيل.",
      },
    },
    payment: {
      title: "الدفع والملاحظات",
      method: "طريقة الدفع",
      cashOnDelivery: "الدفع عند الاستلام",
      onlineUnavailable: "الدفع الإلكتروني غير متاح حاليًا.",
      notes: "ملاحظات الطلب",
      notesPlaceholder: "تعليمات توصيل اختيارية",
    },
    summary: {
      title: "ملخص الطلب",
      subtotal: "الإجمالي الفرعي",
      discount: "الخصم",
      delivery: "التوصيل",
      total: "الإجمالي",
      free: "مجاني",
      pickup: "استلام من المتجر",
      pickupDescription: "سيكون طلبك جاهزًا للاستلام من المتجر.",
    },
    actions: {
      placeOrder: "إتمام الطلب",
      placingOrder: "جارٍ إتمام الطلب...",
    },
    errors: {
      addressesFailed: "تعذر تحميل العناوين.",
      cartFailed: "تعذر تحميل سلة التسوق.",
      previewFailed: "تعذر حساب تفاصيل الطلب.",
      placeOrderFailed: "تعذر إتمام طلبك.",
    },
  },
} as const

type StringValues<T> = {
  [Key in keyof T]: T[Key] extends string ? string : StringValues<T[Key]>
}

export type CheckoutCopy = StringValues<(typeof checkoutCopy)["en"]>
export type CheckoutLocale = keyof typeof checkoutCopy

export function getCheckoutLocale(): CheckoutLocale {
  if (
    typeof document !== "undefined" &&
    document.documentElement.lang.toLowerCase().startsWith("ar")
  ) {
    return "ar"
  }

  return "en"
}

export function getCheckoutCopy(): CheckoutCopy {
  return checkoutCopy[getCheckoutLocale()]
}
