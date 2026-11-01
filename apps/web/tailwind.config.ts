import type { Config } from "tailwindcss";

export default {
  content: ["./src/**/*.{ts,tsx}"],
  theme: {
    extend: {
      fontFamily: {
        sans: ["Segoe UI", "Arial", "Helvetica", "sans-serif"],
      },
    },
  },
  plugins: [],
} satisfies Config;
