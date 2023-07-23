import { writable } from "svelte/store";

let sleepInterval;

const userActiveEvent$ = writable<boolean>(true);
export const userActiveEvent = userActiveEvent$.subscribe;

sleepInterval = setInterval(onLongSleep, 3000);

export function wakeUp() {
    userActiveEvent$.set(true);
    clearInterval(sleepInterval);
    sleepInterval = setInterval(onLongSleep, 3000);
}

function onLongSleep() {
    userActiveEvent$.set(false);
    clearInterval(sleepInterval);
}