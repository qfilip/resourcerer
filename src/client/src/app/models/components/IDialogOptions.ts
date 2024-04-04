export interface IDialogOptions {
    header: string;
    message: string;
    type: 'info' | 'warning';
    buttons: { 
        label: string;
        action: () => void
    }[];
}