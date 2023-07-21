import { writable } from "svelte/store";
import type { IAppUserDto } from "../interfaces/dtos/interfaces";
import { eSection } from "../interfaces/dtos/enums";

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
    const [header, body64String, footer] = jwt.split('.');
    
    const body = JSON.parse(atob(body64String));
    const name = body.sub;
    let permissions: { [key:string]: number } = {};
    
    for (let member in eSection) {
        const section = eSection[member];
        const permissionLevel = body[section] as number;

        if(permissionLevel) {
            permissions[section] = permissionLevel;
        }
    }
    
    window.localStorage.setItem(key, jwt);

    user$.set({ 
        name: name,
        permissions: permissions
    } as IAppUserDto)
}