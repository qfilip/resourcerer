import type { Record } from 'pocketbase';
import type IEntityBase from '../interfaces/dbModels/IEntityBase';
import * as loaderService from '../services/commonUi/loader.service';

export function mapBaseEntity<T extends IEntityBase>(x: Record) {
    const entity = {
        id: x.id,
        created: new Date(x.created),
        updated: new Date(x.updated)
    } as T;

    return entity;
}

export async function handlePromise<T>(fn: () => Promise<T>, message?: string) {
    loaderService.show(message);
    try {
        const data = await fn();
        loaderService.hide();
        return data;
    }
    catch(error) {
        loaderService.hide();
        console.log(error);
    }
}