import { writable } from "svelte/store";
import { IAppUserDto } from "../interfaces/dtos/interfaces";

const user$ = writable<IAppUserDto>();
export const user = user$.subscribe;

export function checkUserLogged() {
    const userString = window.localStorage.getItem('rscr-user');
    if(!userString) {
        return false;
    }

    return true;
}