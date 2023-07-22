import { writable } from "svelte/store";
import type { IAppUserDto } from "../interfaces/dtos/interfaces";
import { ePermissionSection } from "../interfaces/dtos/enums";
import * as pageStore from './commonUi/page.store';
import { addNotification } from "./commonUi/notification.store";
import { eSeverity } from "../interfaces/enums/eSeverity";

const key = 'rscr-user';

const user$ = writable<IAppUserDto>();
const jwt$ = writable<string>();

let cacheControl;

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
    const jwtExpiration = new Date(1000 * body.exp);
    
    let permissions: { [key:string]: number } = {};
    
    for (let member in ePermissionSection) {
        const section = ePermissionSection[member];
        const permissionLevel = body[section] as number;

        if(permissionLevel) {
            permissions[section] = permissionLevel;
        }
    }
    
    window.localStorage.setItem(key, jwt);
    
    clearInterval(cacheControl);
    cacheControl = setInterval(() => {
        window.localStorage.removeItem(key);
        addNotification({text: 'Session expired', severity: eSeverity.Info});
        pageStore.goto.login();
        user$.set(null);
    }, 2000);

    user$.set({ 
        name: name,
        permissions: permissions
    } as IAppUserDto)
}