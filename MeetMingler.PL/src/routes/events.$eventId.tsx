import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/events/$eventId')({
  component: RouteComponent,
})

function RouteComponent() {
  return <div className="flex justify-center items-center h-screen">
    <h1>Event</h1>
  </div>
}
