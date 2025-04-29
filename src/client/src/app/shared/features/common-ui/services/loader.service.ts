import { Injectable, signal } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class LoaderService {
    private _$isLoading = signal<boolean>(false);
    private _$message = signal<string>('');

    $isLoading = this._$isLoading.asReadonly();
    $message = this._$message.asReadonly();

    private tasks = 0;

    constructor() { }

    show(message: string = 'Working...') {
        this.tasks += 1;
        this._$message.set(message);
        this._$isLoading.set(true);
    }

    setMessage(message: string) {
        this._$message.set(message);
    }

    hide() {
        this.tasks -= 1;
        console.log(this.tasks);
        if (this.tasks === 0) {
            this._$isLoading.set(false);
        }
    }
}