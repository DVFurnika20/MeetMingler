import * as React from "react";
import { Outlet, createRootRoute, useMatch } from "@tanstack/react-router";
import { ModeToggle } from "@/components/mode-toggle";
import { Button } from "@/components/ui/button";
import { Search } from "lucide-react";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Toaster } from "@/components/ui/toaster";
import { apiHooks } from "@/client-hooks";
import { schemas } from "@/client";
import { z } from "zod";
import { useNavigate } from "@tanstack/react-router";

export const Route = createRootRoute({
  component: RootComponent,
});

function RootComponent() {
  const { data: user, status } = apiHooks.useGetApiAuthSelf(undefined, {
    retry: false,
  });
  const navigate = useNavigate();
  const isLoginPage = useMatch({ from: "/login", shouldThrow: false });
  const isRegisterPage = useMatch({ from: "/register", shouldThrow: false });

  React.useEffect(() => {
    (async () => {
      if (status === "error" && !isLoginPage && !isRegisterPage) {
        await navigate({ to: "/login" });
      }
    })();
  }, [user, isLoginPage, isRegisterPage, navigate, status]);

  return (
    <React.Fragment>
      <div className="fixed bottom-4 left-4">
        {user ? <AvatarDropdown user={user} /> : <></>}
      </div>
      <div className="fixed bottom-4 right-4">
        <div className="flex space-x-2">
          <Button variant="outline" size="icon">
            <Search />
          </Button>
          <ModeToggle />
        </div>
      </div>
      <Toaster />
      <Outlet />
    </React.Fragment>
  );
}

function AvatarDropdown({ user }: { user: z.infer<typeof schemas.UserVM> }) {
  const { mutate: logout } = apiHooks.usePostApiAuthLogout();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="link" size="icon">
          <Avatar>
            <AvatarFallback>
              {user.firstName[0] + user.lastName[0]}
            </AvatarFallback>
          </Avatar>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        <DropdownMenuLabel className="px-2 py-1.5 text-sm font-normal">
          <div className="flex flex-col space-y-1">
            <p className="text-sm font-medium leading-none">
              {user.firstName + " " + user.lastName}
            </p>
            <p className="text-xs leading-none text-muted-foreground">
              {user.email}
            </p>
          </div>
        </DropdownMenuLabel>
        <DropdownMenuSeparator />
        <DropdownMenuItem>Change Password</DropdownMenuItem>
        <DropdownMenuItem onClick={() => logout(undefined)}>
          Logout
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
