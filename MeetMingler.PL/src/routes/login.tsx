import { schemas } from '@/client';
import { apiHooks } from '@/client-hooks';
import { DefaultTextFormField } from '@/components/default-text-formfield';
import { useTheme } from '@/components/theme-provider';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { useToast } from '@/hooks/use-toast';
import { zodResolver } from '@hookform/resolvers/zod';
import { createFileRoute, Link, useNavigate } from '@tanstack/react-router'
import clsx from 'clsx';
import { FormProvider, useForm } from 'react-hook-form';
import { z } from 'zod';

interface LoginFormProps {
  onSubmit: (values: z.infer<typeof schemas.UserLoginIM>) => void,
  isLoading: boolean
}

const LoginForm: React.FC<LoginFormProps> = (props) => {
  const form = useForm<z.infer<typeof schemas.UserLoginIM>>({
    resolver: zodResolver(schemas.UserLoginIM)
  });

  return <FormProvider {...form}>
    <form onSubmit={form.handleSubmit(props.onSubmit)} className="space-y-4">
      <DefaultTextFormField form={form} name="email" type="email" placeholder="johndoe@example.com" />
      <DefaultTextFormField form={form} name="password" type="password" placeholder="********" />
      <Button disabled={props.isLoading} type="submit" className="w-full">
        {props.isLoading
          ? "Loading..."
          : "Login"}
      </Button>
    </form>
  </FormProvider>;
}

const Login: React.FC = () => {
  const { theme } = useTheme();
  const { toast } = useToast();
  const navigate = useNavigate();

  const { mutate, isLoading } = apiHooks.usePostApiAuthLogin(undefined, {
    async onSuccess(_) {
      await navigate({ to: '/' });
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
    },
  });

  return (
    <div className="flex justify-center items-center h-screen">
      <Card className="w-[450px]">
        <CardHeader>
          <CardTitle>Login</CardTitle>
          <CardDescription>
            If you don't have an account you can register{' '}
            <Link to="/register" className={clsx("underline", theme == "dark" ? "text-white" : "text-black")}>
              there
            </Link>{' '}
            otherwise you can login here
          </CardDescription>
        </CardHeader>
        <CardContent>
          <LoginForm isLoading={isLoading} onSubmit={mutate} />
        </CardContent>
      </Card>
    </div>
  )
}

export const Route = createFileRoute('/login')({
  component: Login,
})
