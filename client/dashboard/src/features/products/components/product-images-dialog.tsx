import { ArrowDown, ArrowUp, ImageIcon, Star, Trash2, Upload } from "lucide-react";
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardFooter } from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";

import { getProductErrorMessage } from "../get-product-error-message";
import { useDeleteProductImage } from "../hooks/use-delete-product-image";
import { useProductImages } from "../hooks/use-product-images";
import { useProductOptions } from "../hooks/use-product-options";
import { useProductVariants } from "../hooks/use-product-variants";
import { useReorderProductImages } from "../hooks/use-reorder-product-images";
import { useSetPrimaryProductImage } from "../hooks/use-set-primary-product-image";
import { useUploadProductImage } from "../hooks/use-upload-product-image";
import { resolveProductImageUrl } from "../product-utils";
import {
  ProductOptionType,
  type ProductDialogEntityProps,
  type ProductImage,
} from "../types";

type GalleryValue = "general" | `${number}`;

export function ProductImagesDialog({
  product,
  open,
  onOpenChange,
}: ProductDialogEntityProps) {
  const { t, i18n } = useTranslation();
  const [galleryValue, setGalleryValue] = useState<GalleryValue>("general");
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [isPrimary, setIsPrimary] = useState(false);

  const { productImagesData, isLoading, error } = useProductImages(
    product.id,
    open
  );
  const { productVariantsData } = useProductVariants(product.id, open);
  const { productOptionsData } = useProductOptions(open);
  const uploadMutation = useUploadProductImage();
  const primaryMutation = useSetPrimaryProductImage();
  const deleteMutation = useDeleteProductImage();
  const reorderMutation = useReorderProductImages();

  const colorOptionIds = useMemo(
    () =>
      new Set(
        (productOptionsData ?? [])
          .filter((option) => option.type === ProductOptionType.Color)
          .map((option) => option.id)
      ),
    [productOptionsData]
  );

  const colorValues = useMemo(() => {
    const valueMap = new Map<
      number,
      { id: number; valueAr: string; valueEn: string }
    >();
    for (const variant of productVariantsData ?? []) {
      for (const option of variant.options) {
        if (colorOptionIds.has(option.optionId)) {
          valueMap.set(option.valueId, {
            id: option.valueId,
            valueAr: option.valueAr,
            valueEn: option.valueEn,
          });
        }
      }
    }
    return Array.from(valueMap.values());
  }, [colorOptionIds, productVariantsData]);

  const selectedColorId =
    galleryValue === "general" ? null : Number(galleryValue);
  const images = productImagesData ?? [];
  const visibleImages = images
    .filter((image) => image.colorOptionValueId === selectedColorId)
    .sort((left, right) => left.displayOrder - right.displayOrder);

  const isMutating =
    uploadMutation.isLoading ||
    primaryMutation.isLoading ||
    deleteMutation.isLoading ||
    reorderMutation.isLoading;
  const mutationError =
    uploadMutation.error ??
    primaryMutation.error ??
    deleteMutation.error ??
    reorderMutation.error;

  const handleUpload = async () => {
    if (!imageFile) return;
    try {
      await uploadMutation.uploadProductImageAsync({
        productId: product.id,
        image: imageFile,
        colorOptionValueId: selectedColorId ?? undefined,
        isPrimary,
      });
      setImageFile(null);
      setIsPrimary(false);
    } catch {
      // Keep input state so the user can retry.
    }
  };

  const handleMove = async (image: ProductImage, direction: -1 | 1) => {
    const index = visibleImages.findIndex((item) => item.id === image.id);
    const nextIndex = index + direction;
    if (index < 0 || nextIndex < 0 || nextIndex >= visibleImages.length) return;

    const orderedIds = visibleImages.map((item) => item.id);
    [orderedIds[index], orderedIds[nextIndex]] = [
      orderedIds[nextIndex],
      orderedIds[index],
    ];

    try {
      await reorderMutation.reorderProductImagesAsync({
        productId: product.id,
        data: {
          colorOptionValueId: selectedColorId,
          imageIds: orderedIds,
        },
      });
    } catch {
      // Display the mutation error.
    }
  };

  const handlePrimary = async (imageId: number) => {
    try {
      await primaryMutation.setPrimaryProductImageAsync({
        productId: product.id,
        imageId,
      });
    } catch {
      // Display the mutation error.
    }
  };

  const handleDelete = async (imageId: number) => {
    try {
      await deleteMutation.deleteProductImageAsync({
        productId: product.id,
        imageId,
      });
    } catch {
      // Display the mutation error.
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="flex max-h-[92vh] max-w-6xl flex-col overflow-hidden">
        <DialogHeader>
          <DialogTitle>{t("products.images.title")}</DialogTitle>
          <DialogDescription>
            {t("products.images.description")}
          </DialogDescription>
        </DialogHeader>

        <div className="grid gap-4 rounded-2xl border p-4 lg:grid-cols-[1fr_1fr_auto_auto] lg:items-end">
          <div className="space-y-2">
            <Label>{t("products.images.gallery")}</Label>
            <Select
              value={galleryValue}
              onValueChange={(value) => setGalleryValue(value as GalleryValue)}
              disabled={isMutating}
            >
              <SelectTrigger className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="general">
                  {t("products.images.generalGallery")}
                </SelectItem>
                {colorValues.map((value) => (
                  <SelectItem key={value.id} value={String(value.id)}>
                    {i18n.resolvedLanguage === "ar"
                      ? value.valueAr
                      : value.valueEn}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor={`product-image-upload-${product.id}`}>
              {t("products.images.file")}
            </Label>
            <Input
              id={`product-image-upload-${product.id}`}
              type="file"
              accept="image/jpeg,image/png,image/webp"
              onChange={(event) => setImageFile(event.target.files?.[0] ?? null)}
              disabled={isMutating}
            />
          </div>

          <div className="flex items-center gap-2 pb-2">
            <Switch
              id={`product-image-primary-${product.id}`}
              checked={isPrimary}
              onCheckedChange={setIsPrimary}
              disabled={isMutating}
            />
            <Label htmlFor={`product-image-primary-${product.id}`}>
              {t("products.images.primary")}
            </Label>
          </div>

          <Button
            onClick={() => void handleUpload()}
            disabled={!imageFile || isMutating}
          >
            <Upload className="size-4" />
            {uploadMutation.isLoading
              ? t("products.images.uploading")
              : t("products.images.upload")}
          </Button>
        </div>

        <p className="text-xs text-muted-foreground">
          {t("products.images.rules")}
        </p>

        <div className="min-h-0 flex-1 overflow-y-auto pe-1">
          {isLoading ? (
            <p className="text-sm text-muted-foreground">{t("common.loading")}</p>
          ) : error ? (
            <p className="text-sm text-destructive">
              {getProductErrorMessage(error, t)}
            </p>
          ) : visibleImages.length === 0 ? (
            <div className="flex min-h-56 flex-col items-center justify-center gap-3 rounded-2xl border border-dashed text-muted-foreground">
              <ImageIcon className="size-10" />
              <p className="text-sm">{t("products.images.empty")}</p>
            </div>
          ) : (
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
              {visibleImages.map((image, index) => (
                <Card key={image.id} size="sm">
                  <CardContent className="px-4">
                    <div className="relative overflow-hidden rounded-xl border bg-muted">
                      <img
                        src={resolveProductImageUrl(image.imageUrl)}
                        alt={t("products.images.imageAlt", { id: image.id })}
                        className="aspect-square w-full object-cover"
                      />
                      {image.isPrimary ? (
                        <Badge className="absolute top-2 start-2">
                          <Star className="size-3" />
                          {t("products.images.primary")}
                        </Badge>
                      ) : null}
                    </div>
                  </CardContent>
                  <CardFooter className="flex flex-wrap gap-2 border-t px-4">
                    <Button
                      variant="outline"
                      size="icon-sm"
                      onClick={() => void handleMove(image, -1)}
                      disabled={index === 0 || isMutating}
                      aria-label={t("products.images.moveUp")}
                    >
                      <ArrowUp className="size-4" />
                    </Button>
                    <Button
                      variant="outline"
                      size="icon-sm"
                      onClick={() => void handleMove(image, 1)}
                      disabled={index === visibleImages.length - 1 || isMutating}
                      aria-label={t("products.images.moveDown")}
                    >
                      <ArrowDown className="size-4" />
                    </Button>
                    {!image.isPrimary ? (
                      <Button
                        variant="outline"
                        size="icon-sm"
                        onClick={() => void handlePrimary(image.id)}
                        disabled={isMutating}
                        aria-label={t("products.images.makePrimary")}
                      >
                        <Star className="size-4" />
                      </Button>
                    ) : null}
                    <Button
                      variant="destructive"
                      size="icon-sm"
                      onClick={() => void handleDelete(image.id)}
                      disabled={isMutating}
                      aria-label={t("common.delete")}
                    >
                      <Trash2 className="size-4" />
                    </Button>
                  </CardFooter>
                </Card>
              ))}
            </div>
          )}
        </div>

        {mutationError ? (
          <p className="text-sm text-destructive">
            {getProductErrorMessage(mutationError, t)}
          </p>
        ) : null}
      </DialogContent>
    </Dialog>
  );
}
