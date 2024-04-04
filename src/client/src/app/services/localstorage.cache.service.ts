import { Injectable } from "@angular/core";
import { CacheFunctions, ICache } from "../models/services/ICache";

@Injectable({
    providedIn: 'root'
})
export class LocalstorageCacheService {
    private _cacheKeys = new Set<string>();

    register<T>(key: string, expiresAfter: number): CacheFunctions {
        if(this._cacheKeys.has(key)) {
            throw `Localstorage cache key ${key} already exists`;
        }

        this._cacheKeys.add(key);

        if(!localStorage.getItem(key)) {
            const now = new Date();
            const cache: ICache = { data: {} as T, expiresAt: now.getTime() };
            localStorage.setItem(key, JSON.stringify(cache));
        }

        return {
            store: <T>(x: T) => {
                this.store(key, x, expiresAfter);
            },
            retrieve: <T>() => {
                return this.retrieve<T>(key);
            },
            clear: () => {
                this.clear(key);
            }
        }
    }

    private store<T>(key: string, data: T, expiresAfter: number) {
        const now = new Date().getTime();

        const cachedData: ICache = {
            data: data,
            expiresAt: new Date(now + expiresAfter).getTime()
        }

        localStorage.setItem(key, JSON.stringify(cachedData));
    }

    private retrieve<T>(key: string): T | null {
        const now = new Date().getTime();
        const cacheString = localStorage.getItem(key) as string;
        const cache = JSON.parse(cacheString) as ICache;
        const expired = (cache.expiresAt) <= now;
        console.log(expired)
        return expired ? null : (cache.data as T);
    }

    private clear(key: string) {
        const cache: ICache = { data: {}, expiresAt: 0 };
        localStorage.setItem(key, JSON.stringify(cache));
    }
}