import { writable } from "svelte/store";
import type { IAppUserDto } from "../interfaces/dtos/interfaces";
import { ePermissionSection } from "../interfaces/dtos/enums";
import * as pageStore from './commonUi/page.store';
import { addNotification } from "./commonUi/notification.store";
import { eSeverity } from "../interfaces/enums/eSeverity";
import { refreshSession } from "../controllers/user.controller";

const key = 'rscr-user';

const jwt$ = writable<string>();
const user$ = writable<IAppUserDto>();

let cacheControl;

export const userChangedEvent = user$.subscribe;
export const jwtChangedEvent = jwt$.subscribe;

let jwt = window.localStorage.getItem(key);
if(jwt) trySetUser(jwt as string);

export function trySetUser(jwt: string) {
    const body = getJwtBody(jwt);
    const name = body.sub;
    
    const jwtExpiration = getJwtExpiration(body);
    const now = new Date().getTime();
    
    const sessionTimeLeft = jwtExpiration - now
    
    if(sessionTimeLeft <= 0) {
        logout();
        return;
    }
    
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
        addNotification({text: 'Session expired', severity: eSeverity.Info});
        logout();
    }, sessionTimeLeft);

    jwt$.set(jwt);
    user$.set({ 
        name: name,
        permissions: permissions
    } as IAppUserDto);
}

export function logout() {
    jwt$.set(null);
    user$.set(null);
    window.localStorage.removeItem(key);
    pageStore.goto.login();
}

export function refreshToken() {
    refreshSession()
}

export function getJwtBody(jwt: string) {
    const [header, body64String, footer] = jwt.split('.');
    return JSON.parse(atob(body64String)) as { [key: string]: any };
}

export function getJwtExpiration(jwtBody: { [key: string]: any }) {
    return new Date(1000 * jwtBody.exp).getTime();
}