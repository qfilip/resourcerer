import { writable } from "svelte/store";
import type { IModalOptions } from "../../interfaces/models/IModalOptions";

export const defaultOptions: IModalOptions = {
    open: false,
    header: 'Warning',
    buttons: []
}

const modalOptions$ = writable<IModalOptions>(defaultOptions);

export const onOpen = modalOptions$.subscribe;

export function open(options?: IModalOptions, onAccept?: () => void) {
    const o: IModalOptions = {
        open: true,
        header: options?.header ?? 'Warning',
        buttons: []
    }

    if(options.buttons && options.buttons.length > 0) {
        o.buttons = options.buttons;
    }
    else {
        onAccept ? o.buttons.push({ text: 'Ok', onClick: () => {
            onAccept();
            modalOptions$.set({open: false} as IModalOptions);
        } }) : noop();

        o.buttons.push({ text: 'Close', onClick: () => {
            modalOptions$.set({open: false} as IModalOptions);
        } });
    }
    modalOptions$.set(o);
}

function noop() {}

export function close() {
    modalOptions$.set({ open: false} as IModalOptions);
}