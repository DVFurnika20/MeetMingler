import { useCallback, useState } from "react";
import { createFileRoute } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  CalendarDays,
  MapPin,
  User,
  Users,
  Info,
  CheckCircle,
} from "lucide-react";
import { MapContainer, TileLayer, Marker } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import L, { LatLng } from "leaflet";
import { useToast } from "@/hooks/use-toast";
import { SidebarProvider } from "@/components/ui/sidebar";
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
import { api } from "@/client";
import { apiHooks } from "@/client-hooks";

L.Icon.Default.mergeOptions({
  iconRetinaUrl:
    "https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon-2x.png",
  iconUrl: "https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon.png",
  shadowUrl: "https://unpkg.com/leaflet@1.7.1/dist/images/marker-shadow.png",
});

export const Route = createFileRoute("/events/$eventId")({
  loader: async ({ params }) => {
    return api.getApiEventGetByIdId({
      params: {
        id: params.eventId,
      },
    });
  },
  notFoundComponent: () => (
    <div className="flex justify-center items-center h-screen">
      <h1>Four O' Four, not found</h1>
    </div>
  ),
  component: RouteComponent,
});

function RouteComponent() {
  const event = Route.useLoaderData();
  // const router = useRouter();
  const { toast } = useToast();
  const metadata = (() => {
    const metadata: Record<string, string> = {};
    for (const item of event.metadata) {
      metadata[item.key] = item.value;
    }

    return metadata;
  })();

  const getCoordinates = useCallback((): LatLng | undefined => {
    if (!("Location" in metadata)) return undefined;

    const res = metadata["Location"]
      .split(",")
      .map((e: string) => parseFloat(e.trim()))
      .filter((e) => !isNaN(e));

    return res.length === 2 ? new LatLng(res[0], res[1]) : undefined;
  }, [metadata]);

  const { mutate: cancelEvent, isLoading: isCancelEventLoading } =
    apiHooks.usePatchApiEventSetCancelledId(
      {
        params: {
          id: event.id,
        },
      },
      {
        onError: (e) => {
          toast({
            title: "Something went wrong with the cancel request",
            description: e.message,
          });
        },
        onSuccess: () => {
          toast({
            title: "Successfully cancelled event!",
          });
        },
      }
    );

  const {
    mutate: registerUserForEvent,
    isLoading: isRegisterUserForEventLoading,
  } = apiHooks.usePostApiEventRegisterUserForEventEventId(
    {
      params: {
        eventId: event.id,
      },
    },
    {
      onError: (e) => {
        toast({
          title: "Something went wrong with the register request",
          description: e.message,
        });
      },
      onSuccess: () => {
        toast({
          title: "Successfully registered for event!",
        });
      },
    }
  );

  const [isCancelDialogOpen, setIsCancelDialogOpen] = useState(false);
  const { toast: showToast } = useToast();

  const handleRegister = async () => {
    // TODO: call api
    registerUserForEvent(undefined);
  };

  const handleUpdateEvent = () => {};

  const handleCancelEvent = () => {
    setIsCancelDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setIsCancelDialogOpen(false);
  };

  const handleConfirmCancel = () => {
    // Logic to cancel the event will go here
    setIsCancelDialogOpen(false);
    cancelEvent(true);
    showToast({
      title: "Success",
      description: "Event successfully cancelled.",
    });
  };

  return (
    <SidebarProvider>
      <div className="container mx-auto p-4 mt-6 font-roboto">
        <div className="flex flex-col md:flex-row gap-10">
          <div className="flex flex-col justify-center items-center w-full md:flex-1">
            {"ImageUrl" in metadata ? (
              <img
                src={metadata["ImageUrl"]}
                alt="Event"
                className="aspect-square object-cover border-4 border-gray-300 rounded-lg shadow-lg mb-4"
              />
            ) : (
              <></>
            )}
            <Card className="w-full">
              <CardHeader>
                <CardTitle className="text-2xl font-bold flex items-center justify-center">
                  <MapPin />
                  <span className="ml-2">Location</span>
                </CardTitle>
              </CardHeader>
              <CardContent>
                {getCoordinates() ? (
                  <MapContainer
                    center={getCoordinates()}
                    zoom={13}
                    scrollWheelZoom={false}
                    className="rounded h-64 z-0"
                  >
                    <TileLayer
                      url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                      attribution="&copy; OpenStreetMap contributors"
                    />
                    <Marker position={getCoordinates()!} />
                  </MapContainer>
                ) : (
                  <p className="text-center">No location data available</p>
                )}
              </CardContent>
            </Card>
          </div>
          <div className="md:ml-4 w-full md:flex-1 flex flex-col justify-center">
            <h1 className="text-5xl font-bold text-center mb-8">
              {event.title}
            </h1>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <EventCard
                title="Date"
                icon={<CalendarDays />}
                content={
                  <span
                    style={{ fontSize: "1.5rem" }}
                  >{`${new Intl.DateTimeFormat().format(
                    new Date(event.startTime)
                  )} - ${new Intl.DateTimeFormat().format(
                    new Date(event.endTime)
                  )}`}</span>
                }
              />
              <EventCard
                title="Status"
                icon={<CheckCircle />}
                content={
                  <span style={{ fontSize: "1.75rem" }}>
                    {event.cancelled ? "Cancelled" : "Active"}
                  </span>
                }
              />
              <EventCard
                title="Participants"
                icon={<Users />}
                content={
                  <span style={{ fontSize: "1.75rem" }}>
                    {/* {event.participantsCount.toString()} */}
                    TODO!
                  </span>
                }
              />
              <EventCard
                title="Created by"
                icon={<User />}
                content={
                  <span style={{ fontSize: "1.75rem" }}>
                    {event.creator.firstName + " " + event.creator.lastName}
                  </span>
                }
              />
              <EventCard
                title="Description"
                icon={<Info />}
                content={event.description}
                className="col-span-1 md:col-span-2"
              />
            </div>
            <div className="mt-4">
              {!event.attending ? (
                <Button
                  onClick={handleRegister}
                  className="w-full bg-blue-500 text-white py-2 rounded-lg shadow-md hover:bg-blue-600 transition duration-300"
                  disabled={isRegisterUserForEventLoading}
                >
                  {isRegisterUserForEventLoading
                    ? "Loading"
                    : "Register for Event"}
                </Button>
              ) : (
                <Button
                  className="w-full bg-gray-500 text-white py-2 rounded-lg shadow-md"
                  disabled
                >
                  You are already registered for this event
                </Button>
              )}
              <div className="flex gap-4 mt-4">
                <Button
                  onClick={handleUpdateEvent}
                  className="w-1/2 bg-green-500 text-white py-2 rounded-lg shadow-md hover:bg-green-600 transition duration-300"
                >
                  Update Event
                </Button>
                <Button
                  onClick={handleCancelEvent}
                  className="w-1/2 bg-red-500 text-white py-2 rounded-lg shadow-md hover:bg-red-600 transition duration-300"
                  disabled={isCancelEventLoading}
                >
                  {isCancelEventLoading ? "Loading" : "Cancel Event"}
                </Button>
              </div>
            </div>
          </div>
        </div>
        {/* <Sidebar>
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
          </Sidebar> */}
        <AlertDialog
          open={isCancelDialogOpen}
          onOpenChange={setIsCancelDialogOpen}
        >
          <AlertDialogContent className="z-50">
            <AlertDialogHeader>
              <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
              <AlertDialogDescription>
                This action cannot be undone. This will permanently delete your
                account and remove your data from our servers.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel onClick={handleCloseDialog}>
                Cancel
              </AlertDialogCancel>
              <AlertDialogAction onClick={handleConfirmCancel}>
                Continue
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </div>
    </SidebarProvider>
  );
}

function EventCard({
  title,
  icon,
  content,
  children,
  className = "",
}: {
  title: string;
  icon: React.ReactNode;
  content: React.ReactNode;
  children?: React.ReactNode;
  className?: string;
}) {
  return (
    <Card className={`w-full mb-4 ${className}`}>
      <CardHeader className="text-center">
        <CardTitle className="text-2xl font-bold flex items-center justify-center">
          {icon}
          <span className="ml-2">{title}</span>
        </CardTitle>
      </CardHeader>
      <CardContent className="text-center">
        <p className="text-lg" style={{ marginTop: "0.5rem" }}>
          {content}
        </p>
        {children}
      </CardContent>
    </Card>
  );
}
