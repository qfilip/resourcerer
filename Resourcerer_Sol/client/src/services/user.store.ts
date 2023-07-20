import { writable } from "svelte/store";
import { IAppUserDto } from "../interfaces/dtos/interfaces";

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
    window.localStorage.setItem(key, jwt);
}