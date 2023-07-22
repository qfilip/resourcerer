import type { IModalButton } from "./IModalButton";

export interface IModalOptions {
    open: boolean;
    header: string;
    buttons: IModalButton[];
}