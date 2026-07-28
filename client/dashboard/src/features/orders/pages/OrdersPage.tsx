import { Search, SlidersHorizontal, X } from "lucide-react"
import { useState } from "react"
import { useTranslation } from "react-i18next"
import { useDebounce } from "use-debounce"

import { EntityPagination } from "@/components/entity-table/entity-pagination"
import { EntityTable } from "@/components/entity-table/entity-table"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { normalizeArabic } from "@/lib/normalize-arabic"

import { useOrderColumns } from "../components/order-columns"
import { getOrderErrorMessage } from "../get-order-error-message"
import { useAdminOrders } from "../hooks/use-admin-orders"
import { getOrderStatusKey, getPaymentStatusKey } from "../order-utils"
import {
  DeliveryMethod,
  OrderSource,
  OrderStatus,
  PaymentMethod,
  PaymentStatus,
} from "../types"

const PAGE_SIZE = 10
const ALL = "all"

export default function OrdersPage() {
  const { t } = useTranslation()
  const columns = useOrderColumns()
  const [search, setSearch] = useState("")
  const [pageNumber, setPageNumber] = useState(1)
  const [status, setStatus] = useState<string>(ALL)
  const [paymentMethod, setPaymentMethod] = useState<string>(ALL)
  const [paymentStatus, setPaymentStatus] = useState<string>(ALL)
  const [deliveryMethod, setDeliveryMethod] = useState<string>(ALL)
  const [orderSource, setOrderSource] = useState<string>(ALL)
  const [fromDate, setFromDate] = useState("")
  const [toDate, setToDate] = useState("")
  const [showFilters, setShowFilters] = useState(true)

  const [debouncedSearch] = useDebounce(normalizeArabic(search), 500)
  const { ordersData, isLoading, error } = useAdminOrders({
    search: debouncedSearch || undefined,
    status: status === ALL ? undefined : (Number(status) as OrderStatus),
    paymentMethod:
      paymentMethod === ALL
        ? undefined
        : (Number(paymentMethod) as PaymentMethod),
    paymentStatus:
      paymentStatus === ALL
        ? undefined
        : (Number(paymentStatus) as PaymentStatus),
    deliveryMethod:
      deliveryMethod === ALL
        ? undefined
        : (Number(deliveryMethod) as DeliveryMethod),
    orderSource:
      orderSource === ALL ? undefined : (Number(orderSource) as OrderSource),
    fromDateUtc: fromDate
      ? new Date(`${fromDate}T00:00:00`).toISOString()
      : undefined,
    toDateUtc: toDate
      ? new Date(`${toDate}T23:59:59.999`).toISOString()
      : undefined,
    pageNumber,
    pageSize: PAGE_SIZE,
  })

  const resetPage = () => setPageNumber(1)
  const clearFilters = () => {
    setStatus(ALL)
    setPaymentMethod(ALL)
    setPaymentStatus(ALL)
    setDeliveryMethod(ALL)
    setOrderSource(ALL)
    setFromDate("")
    setToDate("")
    resetPage()
  }

  const hasFilters =
    status !== ALL ||
    paymentMethod !== ALL ||
    paymentStatus !== ALL ||
    deliveryMethod !== ALL ||
    orderSource !== ALL ||
    Boolean(fromDate) ||
    Boolean(toDate)

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold">{t("orders.page.title")}</h1>
          <p className="text-muted-foreground">
            {t("orders.page.description")}
          </p>
        </div>
        <Button
          variant="outline"
          onClick={() => setShowFilters((value) => !value)}
        >
          <SlidersHorizontal className="size-4" />
          {t("orders.filters.toggle")}
        </Button>
      </div>

      <Card>
        <CardContent className="space-y-4 pt-6">
          <div className="relative">
            <Search className="absolute start-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              value={search}
              onChange={(event) => {
                setSearch(event.target.value)
                resetPage()
              }}
              className="ps-9"
              placeholder={t("orders.searchPlaceholder")}
            />
          </div>

          {showFilters ? (
            <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
              <FilterSelect
                id="order-status-filter"
                label={t("orders.filters.status")}
                value={status}
                onChange={(value) => {
                  setStatus(value)
                  resetPage()
                }}
                allLabel={t("orders.filters.allStatuses")}
                options={Object.values(OrderStatus).map((value) => ({
                  value: String(value),
                  label: t(`orders.status.${getOrderStatusKey(value)}`),
                }))}
              />
              <FilterSelect
                id="payment-method-filter"
                label={t("orders.filters.paymentMethod")}
                value={paymentMethod}
                onChange={(value) => {
                  setPaymentMethod(value)
                  resetPage()
                }}
                allLabel={t("orders.filters.allPaymentMethods")}
                options={[
                  {
                    value: String(PaymentMethod.CashOnDelivery),
                    label: t("orders.paymentMethod.cash_on_delivery"),
                  },
                  {
                    value: String(PaymentMethod.Online),
                    label: t("orders.paymentMethod.online"),
                  },
                ]}
              />
              <FilterSelect
                id="payment-status-filter"
                label={t("orders.filters.paymentStatus")}
                value={paymentStatus}
                onChange={(value) => {
                  setPaymentStatus(value)
                  resetPage()
                }}
                allLabel={t("orders.filters.allPaymentStatuses")}
                options={Object.values(PaymentStatus).map((value) => ({
                  value: String(value),
                  label: t(
                    `orders.paymentStatus.${getPaymentStatusKey(value)}`
                  ),
                }))}
              />
              <FilterSelect
                id="delivery-method-filter"
                label={t("orders.filters.deliveryMethod")}
                value={deliveryMethod}
                onChange={(value) => {
                  setDeliveryMethod(value)
                  resetPage()
                }}
                allLabel={t("orders.filters.allDeliveryMethods")}
                options={[
                  {
                    value: String(DeliveryMethod.HomeDelivery),
                    label: t("orders.deliveryMethod.home_delivery"),
                  },
                  {
                    value: String(DeliveryMethod.StorePickup),
                    label: t("orders.deliveryMethod.store_pickup"),
                  },
                ]}
              />
              <FilterSelect
                id="order-source-filter"
                label={t("orders.filters.source")}
                value={orderSource}
                onChange={(value) => {
                  setOrderSource(value)
                  resetPage()
                }}
                allLabel={t("orders.filters.allSources")}
                options={[
                  {
                    value: String(OrderSource.Website),
                    label: t("orders.source.website"),
                  },
                  {
                    value: String(OrderSource.Store),
                    label: t("orders.source.store"),
                  },
                ]}
              />
              <div className="space-y-2">
                <Label htmlFor="orders-from-date">
                  {t("orders.filters.fromDate")}
                </Label>
                <Input
                  id="orders-from-date"
                  type="date"
                  value={fromDate}
                  onChange={(event) => {
                    setFromDate(event.target.value)
                    resetPage()
                  }}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="orders-to-date">
                  {t("orders.filters.toDate")}
                </Label>
                <Input
                  id="orders-to-date"
                  type="date"
                  value={toDate}
                  min={fromDate || undefined}
                  onChange={(event) => {
                    setToDate(event.target.value)
                    resetPage()
                  }}
                />
              </div>
              <div className="flex items-end">
                <Button
                  type="button"
                  variant="ghost"
                  onClick={clearFilters}
                  disabled={!hasFilters}
                  className="w-full md:w-auto"
                >
                  <X className="size-4" />
                  {t("orders.filters.clear")}
                </Button>
              </div>
            </div>
          ) : null}
        </CardContent>
      </Card>

      <EntityTable
        columns={columns}
        data={ordersData?.items ?? []}
        emptyStateLabel={t("orders.empty")}
      />

      {isLoading ? (
        <p className="text-sm text-muted-foreground">{t("orders.loading")}</p>
      ) : null}
      {error ? (
        <p className="text-sm text-destructive">
          {getOrderErrorMessage(error, t)}
        </p>
      ) : null}

      <EntityPagination
        totalCount={ordersData?.totalCount ?? 0}
        page={ordersData?.pageNumber ?? pageNumber}
        totalPages={ordersData?.totalPages ?? 0}
        totalCountLabel={t("orders.pagination.rows")}
        pageLabel={t("orders.pagination.page")}
        previousLabel={t("orders.pagination.previous")}
        nextLabel={t("orders.pagination.next")}
        onPageChange={setPageNumber}
      />
    </div>
  )
}

function FilterSelect({
  id,
  label,
  value,
  onChange,
  allLabel,
  options,
}: {
  id: string
  label: string
  value: string
  onChange: (value: string) => void
  allLabel: string
  options: Array<{ value: string; label: string }>
}) {
  return (
    <div className="space-y-2">
      <Label htmlFor={id}>{label}</Label>
      <Select value={value} onValueChange={onChange}>
        <SelectTrigger id={id} className="w-full">
          <SelectValue />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value={ALL}>{allLabel}</SelectItem>
          {options.map((option) => (
            <SelectItem key={option.value} value={option.value}>
              {option.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  )
}
