export type PopupColor = 'info' | 'ok' | 'warn' | 'danger';

export type Popup = {
    color: PopupColor;
    header: string;
    text: string;
}