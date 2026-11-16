import { defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    strictPort: true,
  },
  test: {
    environment: "jsdom",
    globals: true,
    setupFiles: ["./vitest.setup.ts"],
    css: false,
    restoreMocks: true,
    // The suite renders whole routes in jsdom; on a loaded machine a single flow can exceed the
    // 5 s default even though it is not hanging.
    testTimeout: 15000,
  },
});
