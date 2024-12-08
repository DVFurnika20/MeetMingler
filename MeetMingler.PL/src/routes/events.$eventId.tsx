import { useState, useEffect } from 'react';
import { createFileRoute } from '@tanstack/react-router';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { FaRegCalendarAlt, FaMapMarkerAlt, FaUser, FaUsers, FaInfoCircle, FaCheckCircle } from 'react-icons/fa';
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import { useToast } from '@/hooks/use-toast';
import {
  Sidebar,
  SidebarContent, SidebarFooter,
  SidebarGroup,
  SidebarGroupLabel,
  SidebarHeader, SidebarMenu, SidebarMenuButton, SidebarMenuItem,
  SidebarProvider
} from '@/components/ui/sidebar';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";


// Correctly configure the default icon options for Leaflet
delete L.Icon.Default.prototype._getIconUrl;

L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon-2x.png',
  iconUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-shadow.png',
});

export const Route = createFileRoute('/events/$eventId')({
  component: RouteComponent,
});

function RouteComponent() {
  const [isRegistered, setIsRegistered] = useState(false);
  const [coordinates] = useState<[number, number]>([51.505, -0.09]); // Default coordinates
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [startTime, setStartTime] = useState(new Date('2023-12-01T10:00:00Z'));
  const [endTime, setEndTime] = useState(new Date('2023-12-03T12:00:00Z'));
  const userId = 'user123'; // Replace with actual user ID
  const eventId = 'event123'; // Replace with actual event ID
  const { toast: showToast } = useToast();

  // Mock data
  const event = {
    title: 'Sample Event',
    creatorName: 'John Pork',
    startTime: new Date('2023-12-01T10:00:00Z'),
    endTime: new Date('2023-12-03T12:00:00Z'),
    location: 'London, United Kingdom',
    description: 'This is a sample event description. It provides detailed information about the event, including the agenda, speakers, and activities planned for the day.',
    imageUrl: 'https://www.eventbookings.com/wp-content/uploads/2018/03/event-ideas-for-party-eventbookings.jpg',
    participantsCount: 42,
    isCancelled: false,
  };

  useEffect(() => {
    const checkRegistration = async () => {
      const registered = await isUserRegistered(userId, eventId);
      setIsRegistered(registered);
    };
    checkRegistration();
  }, [userId, eventId]);

  const handleRegister = async () => {
    if (!isRegistered) {
      //await registerUserForEvent(userId, eventId);
      setIsRegistered(true);
      showToast({ title: 'Success', description: 'You have successfully registered for the event.' });
    }
  };

  const handleUpdateEvent = () => {
    setIsSidebarOpen(true);
    showToast({ title: 'Success', description: 'Event successfully updated.' });
  };

  const handleCancelEvent = () => {
    setIsDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setIsDialogOpen(false);
  };

  const handleConfirmCancel = () => {
    // Logic to cancel the event will go here
    setIsDialogOpen(false);
    showToast({ title: 'Success', description: 'Event successfully cancelled.' });
  };

  return (
      <SidebarProvider>
        <div className="container mx-auto p-4 mt-6 font-roboto">
          <div className="flex flex-col md:flex-row gap-10">
            <div className="flex flex-col justify-center items-center w-full md:flex-1">
              <img src={event.imageUrl} alt="Event" className="w-full h-auto border-4 border-gray-300 rounded-lg shadow-lg mb-4" />
              <EventCard title="Location" icon={<FaMapMarkerAlt />} content={event.location} className="w-full">
                <br />
                <MapContainer center={coordinates} zoom={13} scrollWheelZoom={false} className="h-64 w-full z-0">
                  <TileLayer
                      url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                      attribution='&copy; OpenStreetMap contributors'
                  />
                  <Marker position={coordinates}>
                    <Popup>
                      {event.location}
                    </Popup>
                  </Marker>
                </MapContainer>
              </EventCard>
            </div>
            <div className="md:ml-4 w-full md:flex-1 flex flex-col justify-center">
              <h1 className="text-5xl font-bold text-center mb-8">{event.title}</h1>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <EventCard title="Date" icon={<FaRegCalendarAlt />} content={<span style={{ fontSize: '1.5rem' }}>{`${event.startTime.toLocaleDateString()} - ${event.endTime.toLocaleDateString()}`}</span>} />
                <EventCard title="Status" icon={<FaCheckCircle />} content={<span style={{ fontSize: '1.75rem' }}>{event.isCancelled ? 'Cancelled' : 'Active'}</span>} />
                <EventCard title="Participants" icon={<FaUsers />} content={<span style={{ fontSize: '1.75rem' }}>{event.participantsCount.toString()}</span>} />
                <EventCard title="Created by" icon={<FaUser />} content={<span style={{ fontSize: '1.75rem' }}>{event.creatorName}</span>} />
                <EventCard title="Description" icon={<FaInfoCircle />} content={event.description} className="col-span-1 md:col-span-2" />
              </div>
              <div className="mt-4">
                {!isRegistered ? (
                    <Button onClick={handleRegister} className="w-full bg-blue-500 text-white py-2 rounded-lg shadow-md hover:bg-blue-600 transition duration-300">
                      Register for Event
                    </Button>
                ) : (
                    <Button className="w-full bg-gray-500 text-white py-2 rounded-lg shadow-md" disabled>
                      You are already registered for this event
                    </Button>
                )}
                <div className="flex gap-4 mt-4">
                  <Button onClick={handleUpdateEvent} className="w-1/2 bg-green-500 text-white py-2 rounded-lg shadow-md hover:bg-green-600 transition duration-300">
                    Update Event
                  </Button>
                  <Button onClick={handleCancelEvent} className="w-1/2 bg-red-500 text-white py-2 rounded-lg shadow-md hover:bg-red-600 transition duration-300">
                    Cancel Event
                  </Button>
                </div>
              </div>
            </div>
          </div>
          <Sidebar>
            <SidebarHeader>
              <h2 className="text-lg font-bold">Header</h2>
            </SidebarHeader>
            <SidebarContent>
              <SidebarGroup>
                <SidebarGroupLabel>Group 1</SidebarGroupLabel>
                <SidebarMenu>
                  <SidebarMenuItem>
                    <SidebarMenuButton>Item 1</SidebarMenuButton>
                  </SidebarMenuItem>
                  <SidebarMenuItem>
                    <SidebarMenuButton>Item 2</SidebarMenuButton>
                  </SidebarMenuItem>
                </SidebarMenu>
              </SidebarGroup>
              <SidebarGroup>
                <SidebarGroupLabel>Group 2</SidebarGroupLabel>
                <SidebarMenu>
                  <SidebarMenuItem>
                    <SidebarMenuButton>Item 3</SidebarMenuButton>
                  </SidebarMenuItem>
                  <SidebarMenuItem>
                    <SidebarMenuButton>Item 4</SidebarMenuButton>
                  </SidebarMenuItem>
                </SidebarMenu>
              </SidebarGroup>
            </SidebarContent>
            <SidebarFooter>
              <p className="text-sm">Footer</p>
            </SidebarFooter>
          </Sidebar>
          <AlertDialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
            <AlertDialogContent className="z-50">
              <AlertDialogHeader>
                <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
                <AlertDialogDescription>
                  This action cannot be undone. This will permanently delete your account
                  and remove your data from our servers.
                </AlertDialogDescription>
              </AlertDialogHeader>
              <AlertDialogFooter>
                <AlertDialogCancel onClick={handleCloseDialog}>Cancel</AlertDialogCancel>
                <AlertDialogAction onClick={handleConfirmCancel}>Continue</AlertDialogAction>
              </AlertDialogFooter>
            </AlertDialogContent>
          </AlertDialog>
        </div>
      </SidebarProvider>
  );
}

function EventCard({ title, icon, content, children, className = '' }: { title: string, icon: React.ReactNode, content: React.ReactNode, children?: React.ReactNode, className?: string }) {
  return (
      <Card className={`w-full mb-4 ${className}`}>
        <CardHeader className="text-center">
          <CardTitle className="text-2xl font-bold flex items-center justify-center">
            {icon}
            <span className="ml-2">{title}</span>
          </CardTitle>
        </CardHeader>
        <CardContent className="text-center">
          <p className="text-lg" style={{ marginTop: '0.5rem' }}>{content}</p>
          {children}
        </CardContent>
      </Card>
  );
}