import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// Dev proxy: deliberately absent — each developer points their own
// /api target in their LOCAL.md workflow or browser. See Architecture.md.
export default defineConfig({
  plugins: [vue()],
})
