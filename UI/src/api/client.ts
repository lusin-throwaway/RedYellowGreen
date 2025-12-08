// src/api/client.ts
import axios from "axios";

export const baseUrl = import.meta.env.VITE_API_BASE_URL;

export const api = axios.create({
  baseURL: baseUrl,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true,
});
