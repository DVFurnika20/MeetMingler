import { schemas } from "@/client";
import { apiHooks } from "@/client-hooks";
import { DefaultTextFormField } from "@/components/default-text-formfield";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useToast } from "@/hooks/use-toast";
import { zodResolver } from "@hookform/resolvers/zod";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import {
  FieldValues,
  FormProvider,
  Path,
  useForm,
  UseFormReturn,
} from "react-hook-form";
import { z } from "zod";

import * as React from "react";
import { format } from "date-fns";
import { Calendar as CalendarIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";

interface EventCreateFormProps {
  onSubmit: (values: z.infer<typeof schemas.EventDictionaryIM>) => void;
  isLoading: boolean;
}

export interface DatePickerProps<TSchema extends FieldValues> {
  name: Path<TSchema>;
  form: UseFormReturn<TSchema>;
  disabled: (date: Date) => boolean;
}

export function DatePicker<TSchema extends FieldValues>(
  props: DatePickerProps<TSchema>
) {
  const deCamelCaseify = (input: string): string => {
    const output = input.replace(/([a-z])([A-Z])/g, "$1 $2");

    if (output.length == 0) return output;

    return output[0].toUpperCase() + output.slice(1);
  };

  return (
    <FormField
      control={props.form.control}
      name={props.name}
      render={({ field }) => (
        <FormItem className="w-full flex flex-col">
          <FormLabel>{deCamelCaseify(props.name)}</FormLabel>
          <Popover>
            <PopoverTrigger asChild>
              <FormControl>
                <Button
                  variant={"outline"}
                  className={cn(
                    "pl-3 text-left font-normal",
                    !field.value && "text-muted-foreground"
                  )}
                >
                  {field.value ? (
                    format(field.value, "PPP")
                  ) : (
                    <span>Pick a date</span>
                  )}
                  <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                </Button>
              </FormControl>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="single"
                selected={new Date(field.value)}
                onSelect={(date) => field.onChange(date?.toISOString())}
                disabled={props.disabled}
                initialFocus
              />
            </PopoverContent>
          </Popover>
          <FormMessage />
        </FormItem>
      )}
    />
  );
}

const LoginForm: React.FC<EventCreateFormProps> = (props) => {
  const form = useForm<z.infer<typeof schemas.EventDictionaryIM>>({
    resolver: zodResolver(schemas.EventDictionaryIM),
  });

  return (
    <FormProvider {...form}>
      <form onSubmit={form.handleSubmit(props.onSubmit)} className="space-y-4">
        <DefaultTextFormField
          form={form}
          name="title"
          type="text"
          placeholder="Movie night"
        />
        <DefaultTextFormField
          form={form}
          name="description"
          type="text"
          placeholder="i.e. 'sphinx of black quartz judge my vow'"
        />
        <div className="flex space-x-4">
          <DatePicker
            form={form}
            name="startTime"
            disabled={(date) => date < new Date("1970-01-01")}
          />
          <DatePicker
            form={form}
            name="endTime"
            disabled={(date) => date < new Date("1970-01-01")}
          />
        </div>
        <DefaultTextFormField
          form={form}
          name="metadata.Location"
          namePrettified="Location"
          type="text"
          placeholder="-29.5323842, 137.466814"
        />
        <DefaultTextFormField
          form={form}
          name="metadata.ImageUrl"
          namePrettified="Image URL"
          type="text"
          placeholder="https://example.com/image.png"
        />
        <Button disabled={props.isLoading} type="submit" className="w-full">
          {props.isLoading ? "Loading..." : "Create"}
        </Button>
      </form>
    </FormProvider>
  );
};

const EventCreate: React.FC = () => {
  const { toast } = useToast();
  const navigate = useNavigate();

  const { mutate, isLoading } = apiHooks.usePostApiEventCreate(undefined, {
    async onSuccess(d) {
      await navigate({ to: "/events/$eventId", params: { eventId: d.id } });
    },
    onError(e) {
      // @ts-ignore
      let statusCode = e.response.status as number;
      // @ts-ignore
      let errorText = e.response.data.detail as string;

      if (statusCode >= 400 && statusCode < 500)
        toast({
          title: "Something was wrong with your input",
          description: errorText,
        });
      else if (statusCode >= 500)
        toast({
          title: "Oops! Something went wrong with the server",
        });
    },
  });

  return (
    <div className="flex justify-center items-center h-screen">
      <Card className="w-[450px]">
        <CardHeader>
          <CardTitle>Create a new event</CardTitle>
          <CardDescription>Let's create your exciting event</CardDescription>
        </CardHeader>
        <CardContent>
          <LoginForm isLoading={isLoading} onSubmit={mutate} />
        </CardContent>
      </Card>
    </div>
  );
};

export const Route = createFileRoute("/events/create")({
  component: EventCreate,
});
