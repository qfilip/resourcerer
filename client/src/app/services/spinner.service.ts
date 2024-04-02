import { Injectable, signal } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class SpinnerService {
    private isLoading$ = signal<boolean>(false);
    isLoading = this.isLoading$.asReadonly();

    private message$ = signal<string>('');
    message = this.message$.asReadonly();

    private tasks = 0;

    constructor() { }

    show(message: string = 'Working...') {
        this.tasks += 1;
        this.message$.set(message);
        this.isLoading$.set(true);
    }

    setMessage(message: string) {
        this.message$.set(message);
    }

    hide() {
        this.tasks -= 1;
        if (this.tasks === 0) {
            this.isLoading$.set(false);
        }
    }
}