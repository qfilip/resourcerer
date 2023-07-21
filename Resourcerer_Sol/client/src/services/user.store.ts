import { writable } from "svelte/store";
import type { IAppUserDto } from "../interfaces/dtos/interfaces";

const key = 'rscr-user';

const user$ = writable<IAppUserDto>();
const jwt$ = writable<string>();

export const user = user$.subscribe;
export const jwt = jwt$.subscribe

export function checkUserLogged() {
    const userString = window.localStorage.getItem(key);
    if(!userString) {
        return false;
    }

    return true;
}

export function setUser(jwt: string) {
    const [header, body, footer] = jwt.split('.');
    const name = JSON.parse(atob(body)).sub;
    
    window.localStorage.setItem(key, jwt);
    user$.set({ name: name } as IAppUserDto)
}