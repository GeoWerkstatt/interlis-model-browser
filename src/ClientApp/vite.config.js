import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  base: "",
  plugins: [react()],
  server: {
    proxy: Object.assign(
      {},
      ...["/version", "/search", "/model", "/api"].map((path) => ({
        [path]: {
          target: "https://localhost:7116",
          changeOrigin: true,
          secure: false,
        },
      })),
    ),
    port: 44416,
  },
});
