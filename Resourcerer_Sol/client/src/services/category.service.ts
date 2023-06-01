import { writable } from "svelte/store";

export const selectedCategoryId$ = writable<string>('');