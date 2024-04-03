export type PopupType = 'info' | 'success' | 'warning' | 'error';

export interface IPopup {
    message: string;
    type: PopupType,
    duration: number;
}