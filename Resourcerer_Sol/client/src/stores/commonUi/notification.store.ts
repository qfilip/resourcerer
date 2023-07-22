import { writable } from "svelte/store";
import type { INotification } from "../../interfaces/models/INotification";

const notifications$ = writable<INotification[]>([]);
export const notificationsChangedEvent = notifications$.subscribe;

export const addNotification = (n: INotification | INotification[]) => notifications$.update((xs) => {
    if (Array.isArray(n)) {
        return xs.concat(n);
    }
    else {
        xs.unshift(n);
        return [...xs];
    }
});
export const clearNotifications = () => notifications$.set([]);