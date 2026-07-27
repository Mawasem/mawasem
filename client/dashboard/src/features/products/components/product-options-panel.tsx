import { Edit2, Plus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

import { getProductErrorMessage } from "../get-product-error-message";
import { useProductOptions } from "../hooks/use-product-options";
import {
  ProductOptionType,
  type ProductOption,
  type ProductOptionValue,
} from "../types";
import { ProductOptionDialog } from "./product-option-dialog";
import { ProductOptionValueDialog } from "./product-option-value-dialog";

export function ProductOptionsPanel() {
  const { t, i18n } = useTranslation();
  const { productOptionsData, isLoading, error } = useProductOptions();
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingOption, setEditingOption] = useState<ProductOption | null>(null);
  const [valueDialog, setValueDialog] = useState<{
    option: ProductOption;
    value?: ProductOptionValue;
  } | null>(null);

  const displayOptionName = (option: ProductOption) =>
    i18n.resolvedLanguage === "ar" ? option.nameAr : option.nameEn;
  const displayValueName = (value: ProductOptionValue) =>
    i18n.resolvedLanguage === "ar" ? value.valueAr : value.valueEn;

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h2 className="text-xl font-semibold">{t("products.options.title")}</h2>
          <p className="text-sm text-muted-foreground">
            {t("products.options.description")}
          </p>
        </div>
        <Button onClick={() => setIsCreateOpen(true)}>
          <Plus className="size-4" />
          {t("products.options.create")}
        </Button>
      </div>

      {isLoading ? (
        <p className="text-sm text-muted-foreground">{t("common.loading")}</p>
      ) : error ? (
        <p className="text-sm text-destructive">
          {getProductErrorMessage(error, t)}
        </p>
      ) : (productOptionsData ?? []).length === 0 ? (
        <div className="rounded-2xl border border-dashed p-10 text-center text-sm text-muted-foreground">
          {t("products.options.empty")}
        </div>
      ) : (
        <div className="grid gap-4 lg:grid-cols-2">
          {(productOptionsData ?? []).map((option) => (
            <Card key={option.id}>
              <CardHeader className="border-b">
                <CardTitle>{displayOptionName(option)}</CardTitle>
                <CardDescription>
                  {option.type === ProductOptionType.Color
                    ? t("products.options.color")
                    : t("products.options.standard")}
                </CardDescription>
                <CardAction>
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    onClick={() => setEditingOption(option)}
                    aria-label={t("products.options.edit")}
                  >
                    <Edit2 className="size-4" />
                  </Button>
                </CardAction>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="flex flex-wrap gap-2">
                  {option.values.length === 0 ? (
                    <p className="text-sm text-muted-foreground">
                      {t("products.options.noValues")}
                    </p>
                  ) : (
                    option.values.map((value) => (
                      <Button
                        key={value.id}
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() => setValueDialog({ option, value })}
                      >
                        {displayValueName(value)}
                      </Button>
                    ))
                  )}
                </div>
                <div className="flex items-center justify-between gap-3">
                  <Badge variant="secondary">
                    {t("products.options.valuesCount", {
                      count: option.values.length,
                    })}
                  </Badge>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setValueDialog({ option })}
                  >
                    <Plus className="size-4" />
                    {t("products.options.addValue")}
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      <ProductOptionDialog
        open={isCreateOpen}
        onOpenChange={setIsCreateOpen}
      />
      {editingOption ? (
        <ProductOptionDialog
          option={editingOption}
          open
          onOpenChange={(nextOpen) => {
            if (!nextOpen) setEditingOption(null);
          }}
        />
      ) : null}
      {valueDialog ? (
        <ProductOptionValueDialog
          option={valueDialog.option}
          value={valueDialog.value}
          open
          onOpenChange={(nextOpen) => {
            if (!nextOpen) setValueDialog(null);
          }}
        />
      ) : null}
    </div>
  );
}
