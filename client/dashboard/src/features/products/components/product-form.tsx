import { zodResolver } from "@hookform/resolvers/zod";
import { Plus, Trash2 } from "lucide-react";
import { useFieldArray, useForm, useWatch } from "react-hook-form";
import { useTranslation } from "react-i18next";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { useBrands } from "@/features/brands/hooks/use-brands";
import { useCategories } from "@/features/categories/hooks/use-categories";
import { useCollections } from "@/features/collections/hooks/use-collections";
import { useSeasons } from "@/features/seasons/hooks/use-seasons";
import { CATALOGUE_OPTIONS_PAGE_SIZE } from "@/lib/catalogue-options";

import {
  createProductFormSchema,
  productFormDefaultValues,
  type ProductFormValues,
} from "../schema/product-form-schema";
import type { ProductFormProps } from "../types";

const emptySpecification = {
  nameAr: "",
  nameEn: "",
  valueAr: "",
  valueEn: "",
};

export function ProductForm({
  mode,
  product,
  formId,
  errorMessage,
  onSubmit,
}: ProductFormProps) {
  const { t, i18n } = useTranslation();
  const schema = createProductFormSchema(t);

  const form = useForm<ProductFormValues>({
    resolver: zodResolver(schema),
    defaultValues:
      mode === "edit" && product
        ? {
            nameAr: product.nameAr,
            nameEn: product.nameEn,
            descriptionAr: product.descriptionAr,
            descriptionEn: product.descriptionEn,
            originalPrice: product.originalPrice,
            currentPrice: product.currentPrice,
            slug: product.slug,
            brandId: product.brand.id,
            seasonId: product.season.id,
            categoryIds: product.categories.map((item) => item.id),
            collectionIds: product.collections.map((item) => item.id),
            specifications: product.specifications.map((item) => ({
              nameAr: item.nameAr,
              nameEn: item.nameEn,
              valueAr: item.valueAr,
              valueEn: item.valueEn,
            })),
          }
        : productFormDefaultValues,
  });

  const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: "specifications",
  });

  const selectedSeasonId = useWatch({
    control: form.control,
    name: "seasonId",
  });

  const { data: brandsData, isLoading: isBrandsLoading } = useBrands({
    isActive: true,
    includeDeleted: false,
    pageNumber: 1,
    pageSize: CATALOGUE_OPTIONS_PAGE_SIZE,
  });
  const { data: seasonsData, isLoading: isSeasonsLoading } = useSeasons({
    includeDeleted: false,
    pageNumber: 1,
    pageSize: CATALOGUE_OPTIONS_PAGE_SIZE,
  });
  const { data: categoriesData, isLoading: isCategoriesLoading } =
    useCategories({
      includeDeleted: false,
      pageNumber: 1,
      pageSize: CATALOGUE_OPTIONS_PAGE_SIZE,
    });
  const { data: collectionsData, isLoading: isCollectionsLoading } =
    useCollections({
      includeDeleted: false,
      pageNumber: 1,
      pageSize: CATALOGUE_OPTIONS_PAGE_SIZE,
    });

  const availableCollections = (collectionsData?.items ?? []).filter(
    (collection) =>
      selectedSeasonId <= 0 || collection.seasonId === selectedSeasonId
  );

  const displayName = (item: { nameAr: string; nameEn: string }) =>
    i18n.resolvedLanguage === "ar" ? item.nameAr : item.nameEn;

  return (
    <Form {...form}>
      <form
        id={formId}
        onSubmit={form.handleSubmit(onSubmit)}
        className="space-y-5"
      >
        <Tabs defaultValue="basic">
          <TabsList className="grid h-auto w-full grid-cols-3">
            <TabsTrigger value="basic">{t("products.form.tabs.basic")}</TabsTrigger>
            <TabsTrigger value="relations">
              {t("products.form.tabs.relations")}
            </TabsTrigger>
            <TabsTrigger value="specifications">
              {t("products.form.tabs.specifications")}
            </TabsTrigger>
          </TabsList>

          <TabsContent value="basic" className="space-y-5">
            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="nameAr"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t("products.form.nameAr")}</FormLabel>
                    <FormControl>
                      <Input dir="rtl" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="nameEn"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t("products.form.nameEn")}</FormLabel>
                    <FormControl>
                      <Input dir="ltr" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="descriptionAr"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t("products.form.descriptionAr")}</FormLabel>
                    <FormControl>
                      <Textarea dir="rtl" className="min-h-32" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="descriptionEn"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t("products.form.descriptionEn")}</FormLabel>
                    <FormControl>
                      <Textarea dir="ltr" className="min-h-32" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="grid gap-4 md:grid-cols-3">
              <FormField
                control={form.control}
                name="slug"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t("products.form.slug")}</FormLabel>
                    <FormControl>
                      <Input dir="ltr" placeholder="product-name" {...field} />
                    </FormControl>
                    <FormDescription>
                      {t("products.form.slugDescription")}
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="originalPrice"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t("products.form.originalPrice")}</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min="0.01"
                        step="0.01"
                        value={field.value}
                        onChange={(event) => field.onChange(event.target.valueAsNumber)}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="currentPrice"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t("products.form.currentPrice")}</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min="0.01"
                        step="0.01"
                        value={field.value}
                        onChange={(event) => field.onChange(event.target.valueAsNumber)}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
          </TabsContent>

          <TabsContent value="relations" className="space-y-5">
            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="brandId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t("products.form.brand")}</FormLabel>
                    <Select
                      value={field.value > 0 ? String(field.value) : undefined}
                      onValueChange={(value) => field.onChange(Number(value))}
                      disabled={isBrandsLoading}
                    >
                      <FormControl>
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder={t("products.form.selectBrand")} />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {(brandsData?.items ?? []).map((brand) => (
                          <SelectItem key={brand.id} value={String(brand.id)}>
                            {displayName(brand)}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="seasonId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{t("products.form.season")}</FormLabel>
                    <Select
                      value={field.value > 0 ? String(field.value) : undefined}
                      onValueChange={(value) => {
                        field.onChange(Number(value));
                        form.setValue("collectionIds", [], {
                          shouldDirty: true,
                          shouldValidate: true,
                        });
                      }}
                      disabled={isSeasonsLoading}
                    >
                      <FormControl>
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder={t("products.form.selectSeason")} />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {(seasonsData?.items ?? []).map((season) => (
                          <SelectItem key={season.id} value={String(season.id)}>
                            {displayName(season)}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <FormField
              control={form.control}
              name="categoryIds"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("products.form.categories")}</FormLabel>
                  <div className="grid max-h-56 gap-3 overflow-y-auto rounded-2xl border p-4 sm:grid-cols-2 lg:grid-cols-3">
                    {isCategoriesLoading ? (
                      <p className="text-sm text-muted-foreground">
                        {t("common.loading")}
                      </p>
                    ) : (
                      (categoriesData?.items ?? []).map((category) => {
                        const checkboxId = `product-category-${category.id}`;
                        return (
                          <div
                            key={category.id}
                            className="flex items-center gap-3 rounded-xl border p-3"
                          >
                            <Checkbox
                              id={checkboxId}
                              checked={field.value.includes(category.id)}
                              onCheckedChange={(checked) =>
                                field.onChange(
                                  checked === true
                                    ? [...field.value, category.id]
                                    : field.value.filter((id) => id !== category.id)
                                )
                              }
                            />
                            <Label htmlFor={checkboxId} className="cursor-pointer">
                              {displayName(category)}
                            </Label>
                          </div>
                        );
                      })
                    )}
                  </div>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="collectionIds"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{t("products.form.collections")}</FormLabel>
                  <FormDescription>
                    {t("products.form.collectionsDescription")}
                  </FormDescription>
                  <div className="grid max-h-56 gap-3 overflow-y-auto rounded-2xl border p-4 sm:grid-cols-2 lg:grid-cols-3">
                    {isCollectionsLoading ? (
                      <p className="text-sm text-muted-foreground">
                        {t("common.loading")}
                      </p>
                    ) : availableCollections.length === 0 ? (
                      <p className="text-sm text-muted-foreground">
                        {t("products.form.noCollections")}
                      </p>
                    ) : (
                      availableCollections.map((collection) => {
                        const checkboxId = `product-collection-${collection.id}`;
                        return (
                          <div
                            key={collection.id}
                            className="flex items-center gap-3 rounded-xl border p-3"
                          >
                            <Checkbox
                              id={checkboxId}
                              checked={field.value.includes(collection.id)}
                              onCheckedChange={(checked) =>
                                field.onChange(
                                  checked === true
                                    ? [...field.value, collection.id]
                                    : field.value.filter((id) => id !== collection.id)
                                )
                              }
                            />
                            <Label htmlFor={checkboxId} className="cursor-pointer">
                              {displayName(collection)}
                            </Label>
                          </div>
                        );
                      })
                    )}
                  </div>
                  <FormMessage />
                </FormItem>
              )}
            />
          </TabsContent>

          <TabsContent value="specifications" className="space-y-4">
            <div className="flex items-center justify-between gap-3">
              <div>
                <h3 className="font-semibold">{t("products.form.specifications")}</h3>
                <p className="text-sm text-muted-foreground">
                  {t("products.form.specificationsDescription")}
                </p>
              </div>
              <Button
                type="button"
                variant="outline"
                onClick={() => append(emptySpecification)}
                disabled={fields.length >= 50}
              >
                <Plus className="size-4" />
                {t("products.form.addSpecification")}
              </Button>
            </div>

            {fields.length === 0 ? (
              <div className="rounded-2xl border border-dashed p-8 text-center text-sm text-muted-foreground">
                {t("products.form.noSpecifications")}
              </div>
            ) : (
              <div className="space-y-4">
                {fields.map((field, index) => (
                  <Card key={field.id} size="sm">
                    <CardHeader className="border-b">
                      <CardTitle>
                        {t("products.form.specificationNumber", {
                          number: index + 1,
                        })}
                      </CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-4">
                      <div className="grid gap-4 md:grid-cols-2">
                        <FormField
                          control={form.control}
                          name={`specifications.${index}.nameAr`}
                          render={({ field: inputField }) => (
                            <FormItem>
                              <FormLabel>{t("products.form.specNameAr")}</FormLabel>
                              <FormControl>
                                <Input dir="rtl" {...inputField} />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`specifications.${index}.nameEn`}
                          render={({ field: inputField }) => (
                            <FormItem>
                              <FormLabel>{t("products.form.specNameEn")}</FormLabel>
                              <FormControl>
                                <Input dir="ltr" {...inputField} />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`specifications.${index}.valueAr`}
                          render={({ field: inputField }) => (
                            <FormItem>
                              <FormLabel>{t("products.form.specValueAr")}</FormLabel>
                              <FormControl>
                                <Textarea dir="rtl" {...inputField} />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`specifications.${index}.valueEn`}
                          render={({ field: inputField }) => (
                            <FormItem>
                              <FormLabel>{t("products.form.specValueEn")}</FormLabel>
                              <FormControl>
                                <Textarea dir="ltr" {...inputField} />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                      </div>
                      <Button
                        type="button"
                        variant="destructive"
                        size="sm"
                        onClick={() => remove(index)}
                      >
                        <Trash2 className="size-4" />
                        {t("common.remove")}
                      </Button>
                    </CardContent>
                  </Card>
                ))}
              </div>
            )}

            {form.formState.errors.specifications?.root?.message ? (
              <p className="text-sm text-destructive">
                {form.formState.errors.specifications.root.message}
              </p>
            ) : null}
          </TabsContent>
        </Tabs>

        {errorMessage ? (
          <p className="text-sm text-destructive">{errorMessage}</p>
        ) : null}
      </form>
    </Form>
  );
}
