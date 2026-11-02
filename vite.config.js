import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  build: {
    rollupOptions: {
      output: {
        manualChunks(id) {
          if (id.includes('node_modules')) {
            if (id.includes('/react/') || id.includes('react-dom') || id.includes('scheduler')) return 'react-vendors';
            if (id.includes('/three/')) return 'three';
            if (id.includes('@react-three/fiber')) return 'r3f';
            if (id.includes('@react-three/drei')) return 'drei';
            if (id.includes('@react-three/postprocessing') || id.includes('postprocessing')) return 'postprocessing';
            if (id.includes('gsap')) return 'gsap';
            if (id.includes('framer-motion')) return 'framer-motion';
            return 'vendor';
          }
        },
        chunkFileNames: 'assets/[name]-[hash].js',
        entryFileNames: 'assets/[name]-[hash].js',
        assetFileNames: 'assets/[name]-[hash][extname]'
      }
    }
  }
});
