import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { z } from "zod"
import { schemas } from "@/client"
import { apiHooks } from "@/client-hooks.ts";
import React from "react"

import { Button } from "@/components/ui/button"
import { Form } from "@/components/ui/form"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card.tsx";
import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { DefaultTextFormField } from "@/components/default-text-formfield.tsx";
import { useTheme } from "@/components/theme-provider.tsx";
import { clsx } from "clsx";
import { useToast } from "@/hooks/use-toast.ts";

interface RegisterFormProps {
  onSubmit: (values: z.infer<typeof schemas.UserIM>) => void
}

const RegisterForm: React.FC<RegisterFormProps> = (props) => {
  const form = useForm<z.infer<typeof schemas.UserIM>>({
    resolver: zodResolver(schemas.UserIM)
  });

  return <Form {...form}>
    <form onSubmit={form.handleSubmit(props.onSubmit)} className="space-y-4">
      <DefaultTextFormField form={form} name="firstName" placeholder="John" />
      <DefaultTextFormField form={form} name="lastName" placeholder="Doe" />
      <DefaultTextFormField form={form} name="userName" placeholder="johndoe" />
      <DefaultTextFormField form={form} name="email" type="email" placeholder="johndoe@example.com" />
      <DefaultTextFormField form={form} name="password" type="password" placeholder="********" />
      <Button type="submit" className="w-full">Register</Button>
    </form>
  </Form>;
}

const Register: React.FC = () => {
  const { theme } = useTheme();
  const { toast } = useToast();
  const navigate = useNavigate();

  const { mutate } = apiHooks.usePostApiAuthRegister(undefined, {
    async onSuccess() {
      toast({
        title: "Registration successful, please login"
      })

      await navigate({ to: '/login' });
    },
    onError(e) {
      // @ts-ignore
      let statusCode = e.response.status as number;
      // @ts-ignore
      let errorText = e.response.data.detail as string;

      if (statusCode >= 400 && statusCode < 500)
        toast({
          title: "Something was wrong with your input",
          description: errorText
        })
      else if (statusCode >= 500)
        toast({
          title: "Oops! Something went wrong with the server"
        })
    }
  });

  return (
    <div className="flex justify-center items-center h-screen">
      <Card className="w-[450px]">
        <CardHeader>
          <CardTitle>Register</CardTitle>
          <CardDescription>
            If you have an account you can login{' '}
            <Link to="/login" className={clsx("underline", theme == "dark" ? "text-white" : "text-black")}>
              there
            </Link>{' '}
            otherwise you can register here
          </CardDescription>
        </CardHeader>
        <CardContent>
          <RegisterForm onSubmit={mutate} />
        </CardContent>
      </Card>
    </div>
  )
}

export const Route = createFileRoute('/register')({
  component: Register,
})
