import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import { Popup, PopupColor } from "../models/popup.model";

@Injectable({
    providedIn: 'root'
})
export class PopupService {
    private defaultDuration = 3000;
    private _popup$ = new BehaviorSubject<{x: Popup, duration: number } | null>(null);
    popup$ = this._popup$.asObservable();

    push = (x: Popup, duration = this.defaultDuration) =>
        this._popup$.next({
            x: x,
            duration: duration
        });

    info = (message: string, header = 'Info') =>
        this.pushItem(message, 'info', header);

    ok = (message: string, header = 'Ok') =>
        this.pushItem(message, 'ok', header);

    warn = (message: string, header = 'Warning') =>
        this.pushItem(message, 'warn', header); 

    error = (message: string, header = 'Error') =>
        this.pushItem(message, 'danger', header); 

    private pushItem(message: string, color: PopupColor, header: string) {
        this._popup$.next({
            x: {
                color: color,
                header: header,
                text: message
            },
            duration: this.defaultDuration
        });
    }
}