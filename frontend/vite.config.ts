import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import crypto from 'node:crypto'

if (!globalThis.crypto || !globalThis.crypto.getRandomValues) {
    globalThis.crypto = crypto.webcrypto
}

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
})
