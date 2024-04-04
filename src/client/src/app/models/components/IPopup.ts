export type PopupType = 'info' | 'success' | 'warning' | 'error';

export interface IPopup {
    message: string;
    type: PopupType;
}

export type PopupSnake = { head: IPopup | null, tail: IPopup[] };