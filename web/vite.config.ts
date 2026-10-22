import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// a tela chama /api e o Vite repassa para a API .NET, sem precisar de CORS
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    strictPort: true,
    proxy: {
      '/api': 'http://localhost:5067',
    },
  },
})
