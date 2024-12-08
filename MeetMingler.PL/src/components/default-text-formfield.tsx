import {HTMLInputTypeAttribute} from "react";
import {FormControl, FormField, FormItem, FormLabel, FormMessage} from "@/components/ui/form.tsx";
import {Input} from "@/components/ui/input.tsx";
import {FieldValues, Path, UseFormReturn} from "react-hook-form";

export interface DefaultTextFormFieldProps<TSchema extends FieldValues> {
    name: Path<TSchema>,
    placeholder?: string,
    type?: HTMLInputTypeAttribute,
    form: UseFormReturn<TSchema>
}

export function DefaultTextFormField<TSchema extends FieldValues>(props: DefaultTextFormFieldProps<TSchema>) {
    const deCamelCaseify = (input: string): string => {
        const output = input.replace(/([a-z])([A-Z])/g, '$1 $2');

        if (output.length == 0) return output;

        return output[0].toUpperCase() + output.slice(1);
    }

    return <FormField
        control={props.form.control}
        name={props.name}
        render={({ field }) => (
            <FormItem>
                <FormLabel>{deCamelCaseify(props.name)}</FormLabel>
                <FormControl>
                    <Input placeholder={props.placeholder} type={props.type} {...field} />
                </FormControl>
                <FormMessage />
            </FormItem>
        )}
    />;
}
