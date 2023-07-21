import { writable } from "svelte/store";

const notifications$ = writable<string[]>([]);
export const onNotificationsUpdated = notifications$.subscribe; 

export const addNotification = (m: string | string[]) => notifications$.update((xs) => {
    if(Array.isArray(m)) {
        return xs.concat(m);
    }
    else {
        xs.unshift(m);
        return [...xs];
    }
});
export const clearNotifications = () => notifications$.set([]);