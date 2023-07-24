import { getJwtBody, getJwtExpiration, jwtChangedEvent, logout, refreshToken } from "../user.store";

let jwtExists = false;
let userActive = true;
let sleepInterval;
let sessionDuration: number;

jwtChangedEvent(jwt => {
    if(!jwt) {
        jwtExists = false;
        return;
    };
    
    jwtExists = true;
    userActive = true;
    const jwtBody = getJwtBody(jwt);
    sessionDuration = getJwtExpiration(jwtBody);
    const now = new Date().getTime();

    clearInterval(sleepInterval);
    sleepInterval = setInterval(() => onLongSleep(), (sessionDuration - now) / 2);
});

export function wakeUp() {
    if(!userActive && jwtExists) {
        refreshToken();
    }
}

function onLongSleep() {
    console.log('user sleeps')
    userActive = false;
    clearInterval(sleepInterval);
}
