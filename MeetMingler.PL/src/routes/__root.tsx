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
import { useNavigate } from "@tanstack/react-router";

export const Route = createRootRoute({
  component: RootComponent,
});

function useIsAuthenticated() {
  const { error } = apiHooks.useGetApiAuthSelf();

  return error == null;
}

function RootComponent() {
  const isAuthenticated = useIsAuthenticated();
  const loginMatch = useMatch({ from: "/login", shouldThrow: false });
  const registerMatch = useMatch({ from: "/register", shouldThrow: false });
  const navigate = useNavigate();

  React.useEffect(() => {
    // check if isAuthenticated is false and if page is not register or login with tanstack router
    (async () => {
      if (
        !isAuthenticated &&
        (loginMatch === undefined || registerMatch === undefined)
      ) {
        await navigate({ to: "/login" });
      } else if (
        isAuthenticated &&
        (loginMatch !== undefined || registerMatch !== undefined)
      ) {
        await navigate({ to: "/" });
      }
    })();
  }, [isAuthenticated, loginMatch, registerMatch, navigate]);

  return (
    <React.Fragment>
      <div className="fixed bottom-4 left-4">
        <AvatarDropdown />
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

function AvatarDropdown() {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="link" size="icon">
          <Avatar>
            <AvatarFallback>AM</AvatarFallback>
          </Avatar>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        <DropdownMenuLabel className="px-2 py-1.5 text-sm font-normal">
          <div className="flex flex-col space-y-1">
            <p className="text-sm font-medium leading-none">
              first name last name
            </p>
            <p className="text-xs leading-none text-muted-foreground">
              email@example.com
            </p>
          </div>
        </DropdownMenuLabel>
        <DropdownMenuSeparator />
        <DropdownMenuItem>Toggle theme</DropdownMenuItem>
        <DropdownMenuItem>Change password</DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
