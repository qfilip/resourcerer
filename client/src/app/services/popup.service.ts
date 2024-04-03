import { Injectable, signal } from "@angular/core";
import { IPopup, PopupType } from "../models/components/IPopup";
import { Subject, BehaviorSubject } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class PopupService {
    constructor() { }
    
    private popup$ = signal<IPopup | null>(null);
    popup = this.popup$.asReadonly();
    
    info(message: string, duration = 3000) {
        this.notify(message, 'info', duration);
    }

    success(message: string, duration = 3000) {
        this.notify(message, 'success', duration);
    }

    warning(message: string, duration = 3000) {
        this.notify(message, 'warning', duration);
    }

    error(message: string, duration = 3000) {
        this.notify(message, 'error', duration);
    }

    private notify(message: string, type: PopupType, duration: number) {
        this.popup$.set({
            message: message,
            type: type,
            duration: duration
        } as IPopup);
    }
}