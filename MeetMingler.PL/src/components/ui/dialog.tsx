import { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@shadcn/ui';
import { Button } from '@/components/ui/button';

function Example() {
    const [isDialogOpen, setIsDialogOpen] = useState(false);

    return (
        <div>
            <Button onClick={() => setIsDialogOpen(true)}>Cancel Event</Button>
            <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>Are you sure?</DialogTitle>
                    </DialogHeader>
                    <p>Do you really want to cancel the event? This action cannot be undone.</p>
                    <DialogFooter>
                        <Button variant="secondary" onClick={() => setIsDialogOpen(false)}>Cancel</Button>
                        <Button variant="destructive" onClick={() => {
                            // Logic to cancel the event will go here
                            setIsDialogOpen(false);
                        }}>Yes</Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </div>
    );
}

export default Example;