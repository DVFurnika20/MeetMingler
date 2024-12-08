import * as React from 'react'
import { Outlet, createRootRoute } from '@tanstack/react-router'
import { ModeToggle } from '@/components/mode-toggle'
import { Button } from '@/components/ui/button'
import { Search } from 'lucide-react'
import { Avatar, AvatarFallback } from '@/components/ui/avatar'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger } from '@/components/ui/dropdown-menu'
import { Toaster } from '@/components/ui/toaster'

export const Route = createRootRoute({
    component: RootComponent,
})

function RootComponent() {
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
    )
}

function AvatarDropdown() {
    return <DropdownMenu>
        <DropdownMenuTrigger asChild>
            <Button variant="link" size="icon">
                <Avatar>
                    <AvatarFallback>AM</AvatarFallback>
                </Avatar>
            </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
            <DropdownMenuLabel className='px-2 py-1.5 text-sm font-normal'>
                <div className='flex flex-col space-y-1'>
                    <p className='text-sm font-medium leading-none'>first name last name</p>
                    <p className='text-xs leading-none text-muted-foreground'>email@example.com</p>
                </div>
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem>Toggle theme</DropdownMenuItem>
            <DropdownMenuItem>Change password</DropdownMenuItem>
        </DropdownMenuContent>
    </DropdownMenu>
}
