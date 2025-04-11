export type PopupColor = 'info' | 'ok' | 'warn' | 'error';

export type Popup = {
    color: PopupColor;
    header: string;
    text: string;
}