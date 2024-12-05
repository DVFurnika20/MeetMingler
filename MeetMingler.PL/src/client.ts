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

export const schemas = {
  UserIM,
  UserVM,
  UserLoginIM,
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
    response: z.void(),
  },
]);

export const api = new Zodios(endpoints);

export function createApiClient(baseUrl: string, options?: ZodiosOptions) {
  return new Zodios(baseUrl, endpoints, options);
}
