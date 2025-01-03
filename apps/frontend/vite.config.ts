import { defineConfig } from 'vitest/config';
import { sveltekit } from '@sveltejs/kit/vite';

export default defineConfig({
	plugins: [sveltekit()],
	server: {
		port: process.env.PORT ? parseInt(process.env.PORT) : 5173
	},

	test: {
		include: ['src/**/*.{test,spec}.{js,ts}']
	}
});
