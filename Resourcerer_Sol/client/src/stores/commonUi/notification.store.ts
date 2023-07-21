import { writable } from "svelte/store";

export enum eSeverity {
    Info,
    Warning,
    Error
}

export interface INotification {
    text: string;
    severity: eSeverity;
}

const notifications$ = writable<INotification[]>([]);
export const onNotificationsUpdated = notifications$.subscribe; 

export const addNotification = (n: INotification | INotification[]) => notifications$.update((xs) => {
    if(Array.isArray(n)) {
        return xs.concat(n);
    }
    else {
        xs.unshift(n);
        return [...xs];
    }
});
export const clearNotifications = () => notifications$.set([]);