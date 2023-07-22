import { writable } from "svelte/store";
import type { IAppUserDto } from "../interfaces/dtos/interfaces";
import { ePermissionSection } from "../interfaces/dtos/enums";

const key = 'rscr-user';

const user$ = writable<IAppUserDto>();
const jwt$ = writable<string>();

export const userChangedEvent = user$.subscribe;
export const jwtChangedEvent = jwt$.subscribe;

checkUserLogged();

export function checkUserLogged() {
    const jwtString = window.localStorage.getItem(key);
    if(!jwtString) {
        return false;
    }

    setUser(jwtString);

    return true;
}

export function setUser(jwt: string) {
    const [header, body64String, footer] = jwt.split('.');
    
    const body = JSON.parse(atob(body64String));
    const name = body.sub;
    let permissions: { [key:string]: number } = {};
    
    for (let member in ePermissionSection) {
        const section = ePermissionSection[member];
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