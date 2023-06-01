import { writable } from "svelte/store";

export default interface IPageLoaderOptions {
    open: boolean;
    message: string;
    progress: number;
}

let callCount = 0;
const options$ = writable<IPageLoaderOptions>({ open: false, message: 'loading', progress: null });

export const options = options$.subscribe;

export function show(message?: string, progress?: number) {
    callCount += 1;
    options$.set({
        open: true,
        message: message ?? 'loading',
        progress: progress
    });
}

export function update(message: string, progress: number) {
    options$.set({
        open: true,
        message: message,
        progress: progress
    });
}

export function hide() {
    callCount -= 1;
    if(callCount === 0) {
        options$.set({
            open: false,
            message: 'loading',
            progress: 0
        });
    }
    
}