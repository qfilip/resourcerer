import { Injectable } from "@angular/core";
import { IPopup, PopupSnake, PopupType } from "../models/components/IPopup";
import { BehaviorSubject } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class PopupService {
    constructor() { }
    
    private popupSnake$ = new BehaviorSubject<PopupSnake>({ head: null, tail: [] });
    popupSnake = this.popupSnake$.asObservable();
    
    info(message: string) {
        this.notify(message, 'info');
    }

    success(message: string) {
        this.notify(message, 'success');
    }

    warning(message: string) {
        this.notify(message, 'warning');
    }

    error(message: string) {
        this.notify(message, 'error');
    }

    many(popups: IPopup[]) {
        const [x, xs] = popups;
        const snake = this.popupSnake$.getValue();
        
        snake.head = x;
        
        if(!snake.head) {
            snake.tail.unshift(xs);
        }
        else {
            const tail = [...snake.tail, snake.head ].concat(xs);
            tail.forEach(x => snake.tail.unshift(x));
        }

        this.popupSnake$.next(snake);
    }

    private notify(message: string, type: PopupType) {
        const popup: IPopup = { message: message, type: type }; 
        const snake = this.popupSnake$.getValue();
        
        if(!snake.head) {
            snake.head = popup;
        }
        else {
            snake.tail.unshift(snake.head);
            snake.head = popup;
        }

        this.popupSnake$.next(snake);
    }
}