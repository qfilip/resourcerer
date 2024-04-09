import { Injectable, signal } from "@angular/core";
import { IDialogOptions } from "../models/components/IDialogOptions";

@Injectable({
    providedIn: 'root'
})
export class DialogService {
    private dialogOptions$ = signal<IDialogOptions | null>(null);
    dialogOptions = this.dialogOptions$.asReadonly();

    open(options: IDialogOptions) {
        const o = this.setDefaultOptions(options);
        this.dialogOptions$.set(o);
    }

    close() {
        this.dialogOptions$.set(null);
    }

    private setDefaultOptions(options: IDialogOptions) {
        if (options.buttons && options.buttons.length > 0) {
            options.buttons.forEach(x => {
                if (!x.action) {
                    x.action = () => { }
                }
            });
        }
        const o: IDialogOptions = {
            header: options.header ?? 'Info',
            message: options.message ?? 'Do you wish to proceed?',
            type: options.type ?? 'info',
            buttons: options.buttons ?? [
                { label: 'Ok', action: () => { } }
            ]
        };

        return o;
    }
}