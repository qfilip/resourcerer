import { writable } from "svelte/store";
import type { IModalOptions } from "../../interfaces/models/IModalOptions";

export const defaultOptions: IModalOptions = {
    open: false,
    header: 'Warning',
    buttons: []
}

const modalOptions$ = writable<IModalOptions>(defaultOptions);

export const onOpen = modalOptions$.subscribe;

export function open(options?: IModalOptions) {
    const opts: IModalOptions = {
        open: true,
        header: options?.header ?? 'Warning',
        buttons: options?.buttons ?? []
    }
    modalOptions$.set(opts);
}

export function close() {
    modalOptions$.set({ open: false} as IModalOptions);
}