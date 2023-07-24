import { writable } from "svelte/store";
import type { IAppUserDto } from "../interfaces/dtos/interfaces";
import { ePermissionSection } from "../interfaces/dtos/enums";
import * as pageStore from './commonUi/page.store';
import { addNotification } from "./commonUi/notification.store";
import { eSeverity } from "../interfaces/enums/eSeverity";
import * as sleepStore from "./commonUi/sleep.store";

const key = 'rscr-user';

const user$ = writable<IAppUserDto>();
const jwt$ = writable<string>();

let cacheControl;
let countInterval;
let userActive = false;

export const userChangedEvent = user$.subscribe;
export const jwtChangedEvent = jwt$.subscribe;

sleepStore.userActiveEvent(x => userActive = x);

checkUserLogged();

export function checkUserLogged() {
    const jwtString = window.localStorage.getItem(key);
    if(!jwtString) {
        return false;
    }

    return trySetUser(jwtString);
}

export function trySetUser(jwt: string) {
    clearInterval(countInterval);
    countInterval = setInterval(() => console.log('passed'), 3000);
    const [header, body64String, footer] = jwt.split('.');
    
    const body = JSON.parse(atob(body64String));
    const name = body.sub;
    
    const jwtExpiration = new Date(1000 * body.exp).getTime();
    const now = new Date().getTime();
    
    const sessionTimeLeft = jwtExpiration - now
    const sessionDurationFifth = jwtExpiration / 5;
    
    if(sessionTimeLeft <= 0) {
        logout();
        return false;
    }
    else if(userActive && sessionTimeLeft < sessionDurationFifth) {
        // what if less than 10 mins left in session? Call refresh token
        console.log('call refresh');
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

    return true;
}

export function logout() {
    window.localStorage.removeItem(key);
    pageStore.goto.login();
    user$.set(null);
}