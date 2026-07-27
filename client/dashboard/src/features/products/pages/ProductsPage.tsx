import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useDebounce } from "use-debounce";

import { EntityManagementPage } from "@/components/entity-management/EntityManagementPage";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useBrands } from "@/features/brands/hooks/use-brands";
import { useCategories } from "@/features/categories/hooks/use-categories";
import { useCollections } from "@/features/collections/hooks/use-collections";
import { useSeasons } from "@/features/seasons/hooks/use-seasons";
import { CATALOGUE_OPTIONS_PAGE_SIZE } from "@/lib/catalogue-options";
import { normalizeArabic } from "@/lib/normalize-arabic";

import { ProductDialog } from "../components/product-dialog";
import { ProductOptionsPanel } from "../components/product-options-panel";
import { useProductColumns } from "../components/product-columns";
import { getProductErrorMessage } from "../get-product-error-message";
import { useProducts } from "../hooks/use-products";

const ALL = "all";
type BooleanFilter = typeof ALL | "true" | "false";

export default function ProductsPage() {
  const { t, i18n } = useTranslation();
  const columns = useProductColumns();
  const [searchInput, setSearchInput] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [includeDeleted, setIncludeDeleted] = useState(false);
  const [brandId, setBrandId] = useState(ALL);
  const [seasonId, setSeasonId] = useState(ALL);
  const [categoryId, setCategoryId] = useState(ALL);
  const [collectionId, setCollectionId] = useState(ALL);
  const [publication, setPublication] = useState<BooleanFilter>(ALL);
  const [featured, setFeatured] = useState<BooleanFilter>(ALL);
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);

  const [debouncedSearch] = useDebounce(normalizeArabic(searchInput), 500);

  const { productsData, isLoading, error } = useProducts({
    search: debouncedSearch || undefined,
    brandId: brandId === ALL ? undefined : Number(brandId),
    seasonId: seasonId === ALL ? undefined : Number(seasonId),
    categoryId: categoryId === ALL ? undefined : Number(categoryId),
    collectionId: collectionId === ALL ? undefined : Number(collectionId),
    isPublished: publication === ALL ? undefined : publication === "true",
    isFeatured: featured === ALL ? undefined : featured === "true",
    includeDeleted,
    pageNumber,
    pageSize: 10,
  });

  const { data: brandsData } = useBrands({
    isActive: true,
    includeDeleted: false,
    pageNumber: 1,
    pageSize: CATALOGUE_OPTIONS_PAGE_SIZE,
  });
  const { data: seasonsData } = useSeasons({
    includeDeleted: false,
    pageNumber: 1,
    pageSize: CATALOGUE_OPTIONS_PAGE_SIZE,
  });
  const { data: categoriesData } = useCategories({
    includeDeleted: false,
    pageNumber: 1,
    pageSize: CATALOGUE_OPTIONS_PAGE_SIZE,
  });
  const { data: collectionsData } = useCollections({
    includeDeleted: false,
    pageNumber: 1,
    pageSize: CATALOGUE_OPTIONS_PAGE_SIZE,
  });

  const resetPage = () => setPageNumber(1);
  const displayName = (item: { nameAr: string; nameEn: string }) =>
    i18n.resolvedLanguage === "ar" ? item.nameAr : item.nameEn;

  const currentPage = productsData?.pageNumber ?? pageNumber;
  const totalPages = productsData?.totalPages ?? 0;

  return (
    <Tabs defaultValue="products" className="space-y-2">
      <TabsList>
        <TabsTrigger value="products">{t("products.tabs.products")}</TabsTrigger>
        <TabsTrigger value="options">{t("products.tabs.options")}</TabsTrigger>
      </TabsList>

      <TabsContent value="products">
        <EntityManagementPage
          title={t("products.page.title")}
          description={t("products.page.description")}
          search={searchInput}
          onSearch={(value) => {
            setSearchInput(value);
            resetPage();
          }}
          searchPlaceholder={t("products.searchPlaceholder")}
          includeDeleted={includeDeleted}
          onIncludeDeletedChange={(value) => {
            setIncludeDeleted(value);
            resetPage();
          }}
          includeDeletedLabel={t("products.filters.includeDeleted")}
          includeDeletedSwitchId="include-deleted-products"
          buttonLabel={t("products.actions.create")}
          onCreate={() => setIsCreateDialogOpen(true)}
          columns={columns}
          data={productsData?.items ?? []}
          emptyStateLabel={t("products.empty")}
          loading={isLoading}
          loadingLabel={t("products.loading")}
          error={error}
          errorRenderer={(nextError) => getProductErrorMessage(nextError, t)}
          pagination={{
            totalCount: productsData?.totalCount ?? 0,
            page: currentPage,
            totalPages,
            totalCountLabel: t("products.pagination.rows"),
            pageLabel: t("products.pagination.page"),
            previousLabel: t("products.pagination.previous"),
            nextLabel: t("products.pagination.next"),
            onPageChange: (nextPage) => {
              const maxPage = totalPages > 0 ? totalPages : 1;
              if (nextPage >= 1 && nextPage <= maxPage && nextPage !== currentPage) {
                setPageNumber(nextPage);
              }
            },
          }}
          filtersSlot={
            <div className="grid w-full gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6">
              <FilterSelect
                id="products-brand-filter"
                label={t("products.filters.brand")}
                value={brandId}
                onValueChange={(value) => {
                  setBrandId(value);
                  resetPage();
                }}
                allLabel={t("products.filters.allBrands")}
                items={(brandsData?.items ?? []).map((item) => ({
                  value: String(item.id),
                  label: displayName(item),
                }))}
              />
              <FilterSelect
                id="products-season-filter"
                label={t("products.filters.season")}
                value={seasonId}
                onValueChange={(value) => {
                  setSeasonId(value);
                  setCollectionId(ALL);
                  resetPage();
                }}
                allLabel={t("products.filters.allSeasons")}
                items={(seasonsData?.items ?? []).map((item) => ({
                  value: String(item.id),
                  label: displayName(item),
                }))}
              />
              <FilterSelect
                id="products-category-filter"
                label={t("products.filters.category")}
                value={categoryId}
                onValueChange={(value) => {
                  setCategoryId(value);
                  resetPage();
                }}
                allLabel={t("products.filters.allCategories")}
                items={(categoriesData?.items ?? []).map((item) => ({
                  value: String(item.id),
                  label: displayName(item),
                }))}
              />
              <FilterSelect
                id="products-collection-filter"
                label={t("products.filters.collection")}
                value={collectionId}
                onValueChange={(value) => {
                  setCollectionId(value);
                  resetPage();
                }}
                allLabel={t("products.filters.allCollections")}
                items={(collectionsData?.items ?? [])
                  .filter(
                    (item) => seasonId === ALL || item.seasonId === Number(seasonId)
                  )
                  .map((item) => ({
                    value: String(item.id),
                    label: displayName(item),
                  }))}
              />
              <FilterSelect
                id="products-publication-filter"
                label={t("products.filters.publication")}
                value={publication}
                onValueChange={(value) => {
                  setPublication(value as BooleanFilter);
                  resetPage();
                }}
                allLabel={t("products.filters.allPublication")}
                items={[
                  { value: "true", label: t("products.status.published") },
                  { value: "false", label: t("products.status.draft") },
                ]}
              />
              <FilterSelect
                id="products-featured-filter"
                label={t("products.filters.featured")}
                value={featured}
                onValueChange={(value) => {
                  setFeatured(value as BooleanFilter);
                  resetPage();
                }}
                allLabel={t("products.filters.allFeatured")}
                items={[
                  { value: "true", label: t("products.filters.featuredOnly") },
                  { value: "false", label: t("products.filters.notFeatured") },
                ]}
              />
            </div>
          }
        >
          <ProductDialog
            mode="create"
            open={isCreateDialogOpen}
            onOpenChange={setIsCreateDialogOpen}
          />
        </EntityManagementPage>
      </TabsContent>

      <TabsContent value="options">
        <ProductOptionsPanel />
      </TabsContent>
    </Tabs>
  );
}

interface FilterSelectProps {
  id: string;
  label: string;
  value: string;
  onValueChange: (value: string) => void;
  allLabel: string;
  items: Array<{ value: string; label: string }>;
}

function FilterSelect({
  id,
  label,
  value,
  onValueChange,
  allLabel,
  items,
}: FilterSelectProps) {
  return (
    <div className="min-w-0 space-y-2">
      <Label htmlFor={id}>{label}</Label>
      <Select value={value} onValueChange={onValueChange}>
        <SelectTrigger id={id} className="w-full">
          <SelectValue />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value={ALL}>{allLabel}</SelectItem>
          {items.map((item) => (
            <SelectItem key={item.value} value={item.value}>
              {item.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}
