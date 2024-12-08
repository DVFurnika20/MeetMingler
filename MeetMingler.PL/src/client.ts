import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const UserIM = z
  .object({
    firstName: z.string().nullable(),
    lastName: z.string().nullable(),
    email: z.string().nullable(),
    userName: z.string().nullable(),
    password: z.string().nullable(),
  })
  .partial();
const UserVM = z
  .object({
    id: z.string().uuid(),
    firstName: z.string().nullable(),
    lastName: z.string().nullable(),
    email: z.string().nullable(),
    userName: z.string().nullable(),
  })
  .partial();
const UserLoginIM = z.object({
  email: z.string().min(1),
  password: z.string().min(1),
});
const EventMetadataIM = z.object({
  key: z.string().min(1),
  value: z.string().min(1),
});
const EventIM = z.object({
  title: z.string().min(1),
  description: z.string().min(1),
  startTime: z.string().datetime({ offset: true }),
  endTime: z.string().datetime({ offset: true }),
  metadata: z.array(EventMetadataIM),
});
const EventMetadataVM = z.object({
  key: z.string().min(1),
  value: z.string().min(1),
});
const EventVM = z.object({
  title: z.string().min(1),
  description: z.string().min(1),
  startTime: z.string().datetime({ offset: true }),
  endTime: z.string().datetime({ offset: true }),
  id: z.string().uuid().optional(),
  creator: UserVM.optional(),
  cancelled: z.boolean().optional(),
  metadata: z.array(EventMetadataVM).nullish(),
});
const EventFilter = z.object({
  includeMetadataKeys: z.array(z.string()).nullish(),
  metadataFilters: z.record(z.string()).nullish(),
  textSearch: z.string().min(1),
});
const EventVMBaseCollectionVM = z.object({
  items: z.array(EventVM),
  count: z.number().int(),
});
const EventUM = z.object({
  title: z.string().min(1),
  description: z.string().min(1),
  startTime: z.string().datetime({ offset: true }),
  endTime: z.string().datetime({ offset: true }),
});

export const schemas = {
  UserIM,
  UserVM,
  UserLoginIM,
  EventMetadataIM,
  EventIM,
  EventMetadataVM,
  EventVM,
  EventFilter,
  EventVMBaseCollectionVM,
  EventUM,
};

const endpoints = makeApi([
  {
    method: "post",
    path: "/api/Auth/Login",
    alias: "postApiAuthLogin",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: UserLoginIM,
      },
    ],
    response: z.void(),
  },
  {
    method: "post",
    path: "/api/Auth/Logout",
    alias: "postApiAuthLogout",
    requestFormat: "json",
    response: z.void(),
  },
  {
    method: "post",
    path: "/api/Auth/Register",
    alias: "postApiAuthRegister",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: UserIM,
      },
    ],
    response: UserVM,
  },
  {
    method: "get",
    path: "/api/Auth/Self",
    alias: "getApiAuthSelf",
    requestFormat: "json",
    response: UserVM,
  },
  {
    method: "post",
    path: "/api/Event/AddMetadata/:eventId",
    alias: "postApiEventAddMetadataEventId",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: EventMetadataIM,
      },
      {
        name: "eventId",
        type: "Path",
        schema: z.string().uuid(),
      },
    ],
    response: EventVM,
  },
  {
    method: "post",
    path: "/api/Event/Create",
    alias: "postApiEventCreate",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: EventIM,
      },
    ],
    response: EventVM,
  },
  {
    method: "delete",
    path: "/api/Event/Delete/:id",
    alias: "deleteApiEventDeleteId",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
    ],
    response: z.void(),
  },
  {
    method: "delete",
    path: "/api/Event/DeleteMetadata/:eventId/:key",
    alias: "deleteApiEventDeleteMetadataEventIdKey",
    requestFormat: "json",
    parameters: [
      {
        name: "eventId",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "key",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
  },
  {
    method: "post",
    path: "/api/Event/GetAllPaginatedAndFiltered",
    alias: "postApiEventGetAllPaginatedAndFiltered",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: EventFilter,
      },
      {
        name: "Page",
        type: "Query",
        schema: z.number().int().gte(1).lte(2147483647),
      },
      {
        name: "PageSize",
        type: "Query",
        schema: z.number().int().gte(2).lte(15),
      },
      {
        name: "SortByColumn",
        type: "Query",
        schema: z.string().optional(),
      },
      {
        name: "Order",
        type: "Query",
        schema: z.enum(["Ascending", "Descending"]).optional(),
      },
    ],
    response: EventVMBaseCollectionVM,
  },
  {
    method: "get",
    path: "/api/Event/GetAttendees/:eventId",
    alias: "getApiEventGetAttendeesEventId",
    requestFormat: "json",
    parameters: [
      {
        name: "eventId",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "Page",
        type: "Query",
        schema: z.number().int().gte(1).lte(2147483647),
      },
      {
        name: "PageSize",
        type: "Query",
        schema: z.number().int().gte(2).lte(15),
      },
      {
        name: "SortByColumn",
        type: "Query",
        schema: z.string().optional(),
      },
      {
        name: "Order",
        type: "Query",
        schema: z.enum(["Ascending", "Descending"]).optional(),
      },
    ],
    response: UserVM,
  },
  {
    method: "get",
    path: "/api/Event/GetByCreator/:creatorId",
    alias: "getApiEventGetByCreatorCreatorId",
    requestFormat: "json",
    parameters: [
      {
        name: "creatorId",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "includeMetadataKeys",
        type: "Query",
        schema: z.array(z.string()).optional(),
      },
      {
        name: "Page",
        type: "Query",
        schema: z.number().int().gte(1).lte(2147483647),
      },
      {
        name: "PageSize",
        type: "Query",
        schema: z.number().int().gte(2).lte(15),
      },
      {
        name: "SortByColumn",
        type: "Query",
        schema: z.string().optional(),
      },
      {
        name: "Order",
        type: "Query",
        schema: z.enum(["Ascending", "Descending"]).optional(),
      },
    ],
    response: z.array(EventVM),
  },
  {
    method: "get",
    path: "/api/Event/GetById/:id",
    alias: "getApiEventGetByIdId",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
    ],
    response: EventVM,
  },
  {
    method: "get",
    path: "/api/Event/GetDates",
    alias: "getApiEventGetDates",
    requestFormat: "json",
    parameters: [
      {
        name: "startDateRange",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "endDateRange",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
    ],
    response: z.array(z.string().datetime({ offset: true })),
  },
  {
    method: "get",
    path: "/api/Event/GetDistinctMetadataValues/:metadataKey",
    alias: "getApiEventGetDistinctMetadataValuesMetadataKey",
    requestFormat: "json",
    parameters: [
      {
        name: "metadataKey",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(z.string()),
  },
  {
    method: "get",
    path: "/api/Event/GetSelfAttendance",
    alias: "getApiEventGetSelfAttendance",
    requestFormat: "json",
    parameters: [
      {
        name: "Page",
        type: "Query",
        schema: z.number().int().gte(1).lte(2147483647),
      },
      {
        name: "PageSize",
        type: "Query",
        schema: z.number().int().gte(2).lte(15),
      },
      {
        name: "SortByColumn",
        type: "Query",
        schema: z.string().optional(),
      },
      {
        name: "Order",
        type: "Query",
        schema: z.enum(["Ascending", "Descending"]).optional(),
      },
    ],
    response: EventVM,
  },
  {
    method: "post",
    path: "/api/Event/RegisterUserForEvent/:eventId",
    alias: "postApiEventRegisterUserForEventEventId",
    requestFormat: "json",
    parameters: [
      {
        name: "eventId",
        type: "Path",
        schema: z.string().uuid(),
      },
    ],
    response: z.void(),
  },
  {
    method: "patch",
    path: "/api/Event/SetCancelled/:id",
    alias: "patchApiEventSetCancelledId",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.boolean(),
      },
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
    ],
    response: z.void(),
  },
  {
    method: "put",
    path: "/api/Event/Update/:eventId",
    alias: "putApiEventUpdateEventId",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: EventUM,
      },
      {
        name: "eventId",
        type: "Path",
        schema: z.string().uuid(),
      },
    ],
    response: EventVM,
  },
]);

export const api = new Zodios(endpoints, {
  axiosConfig: {
    baseURL: import.meta.env.VITE_API_BASE_URL
  }
});

export function createApiClient(baseUrl: string, options?: ZodiosOptions) {
  return new Zodios(baseUrl, endpoints, options);
}
