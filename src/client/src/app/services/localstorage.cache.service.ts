import { Injectable } from "@angular/core";
import { CacheFunctions, ICache } from "../models/services/ICache";
import { Observable, tap } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class LocalstorageCacheService {
    private _cache = new Map<string, ICache>();

    register<T>(key: string, expiresAfter: number): CacheFunctions {
        if(this._cache.has(key)) {
            throw `Localstorage cache key ${key} already exists`;
        }

        const now = new Date();
        this._cache.set(key, { data: {} as T, storedAt: now.getTime(), expiresAfter: 0 } as ICache);

        return {
            store: <T>(x: T) => {
                this.store(key, x, expiresAfter);
            },
            retrieve: <T>() => {
                return this.retrieve<T>(key);
            }
        }
    }

    private store<T>(key: string, data: T, expiresAfter: number) {
        const now = new Date().getTime();
        const cachedData: ICache = {
            data: data,
            storedAt: now,
            expiresAfter: expiresAfter
        }

        this._cache.set(key, cachedData);
    }

    private retrieve<T>(key: string): T | null {
        const now = new Date().getTime();
        const cache = this._cache.get(key) as ICache;
        const expired = (cache.storedAt + cache.expiresAfter) <= now;

        return expired ? null : (cache.data as T);
    }
}