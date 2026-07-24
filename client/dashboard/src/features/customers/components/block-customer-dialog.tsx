import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";

import { EntityDialog } from "@/components/entity-dialog/entity-dialog";
import { EntityDialogFooter } from "@/components/entity-dialog/entity-dialog-footer";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Textarea } from "@/components/ui/textarea";

import { useBlockCustomer } from "../hooks/use-block-customer";
import {
  blockCustomerDefaultValues,
  createBlockCustomerFormSchema,
  type BlockCustomerFormValues,
} from "../schema/block-customer-schema";
import type { BlockCustomerDialogProps } from "../types";

export function BlockCustomerDialog({
  customer,
  open,
  onOpenChange,
}: BlockCustomerDialogProps) {
  const { t, i18n } = useTranslation();

  const blockCustomerMutation = useBlockCustomer();

  const blockCustomerFormSchema =
    createBlockCustomerFormSchema(t);

  const form = useForm<BlockCustomerFormValues>({
    resolver: zodResolver(blockCustomerFormSchema),
    defaultValues: blockCustomerDefaultValues,
  });

  useEffect(() => {
    if (open) {
      form.reset(blockCustomerDefaultValues);
    }
  }, [form, open]);

  const errorMessage =
    blockCustomerMutation.error instanceof Error
      ? blockCustomerMutation.error.message
      : null;

  const formId = `block-customer-form-${customer.id}`;

  const handleSubmit = async (
    values: BlockCustomerFormValues
  ) => {
    try {
      await blockCustomerMutation.blockCustomerAsync({
        customerId: customer.id,
        reason: values.reason,
      });

      onOpenChange(false);
    } catch {
      // Keep dialog open and show mutation error.
    }
  };

  return (
    <EntityDialog
      open={open}
      onOpenChange={onOpenChange}
      title={t("customers.blockDialog.title")}
      description={t("customers.blockDialog.description")}
    >
      <div className="space-y-5">
        <p className="text-sm font-medium">
          {i18n.resolvedLanguage === "ar"
            ? customer.fullNameAr
            : customer.fullNameEn}
        </p>

        <Form {...form}>
          <form
            id={formId}
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-5"
          >
            <FormField
              control={form.control}
              name="reason"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>
                    {t("customers.blockDialog.reasonLabel")}
                  </FormLabel>

                  <FormControl>
                    <Textarea
                      placeholder={t("customers.blockDialog.reasonPlaceholder")}
                      {...field}
                    />
                  </FormControl>

                  <FormMessage />
                </FormItem>
              )}
            />

            {errorMessage ? (
              <p className="text-sm text-destructive">
                {errorMessage}
              </p>
            ) : null}
          </form>
        </Form>

        <EntityDialogFooter
          mode="create"
          formId={formId}
          isLoading={blockCustomerMutation.isLoading}
          onCancel={() => onOpenChange(false)}
          createLabel={t("customers.actions.block")}
          createLoadingLabel={t("customers.actions.blocking")}
          cancelLabel={t("common.cancel")}
        />
      </div>
    </EntityDialog>
  );
}
