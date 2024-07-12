import { Injectable } from "@angular/core";
import { CacheFunctions, ICache } from "../models/services/ICache";
import { CacheService } from "./cache.service";
import { Observable, of, tap } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class LocalstorageCacheService extends CacheService {
    private _cacheKeys = new Set<string>();

    register<T>(key: string, fetchFn: () => Observable<T>, expiresAfter: number): CacheFunctions<T> {
        if(this._cacheKeys.has(key)) {
            throw `Localstorage cache key ${key} already exists`;
        }

        this._cacheKeys.add(key);

        if(!localStorage.getItem(key)) {
            this.setCache<T>(key, {} as T, 0);
        }

        return {
            retrieve: () => {
                return this.retrieve<T>(key, expiresAfter, fetchFn);
            },
            getCache: () => {
                return this.getCache(key);
            },
            setCache: <T>(data: T) => {
                return this.setCache(key, data, expiresAfter);
            },
            clear: () => {
                this.clear(key);
            }
        }
    }

    protected retrieve<T>(key: string, expiresAfter: number, fetchFn: () => Observable<T>): Observable<T> {
        const expired = this.hasExpired(key);

        return expired
          ? fetchFn()
              .pipe(
                tap(x => this.setCache<T>(key, x, expiresAfter))
            )
          : of(this.getCache(key).data as T);
    }

    protected override getCache(key: string): ICache {
        const cacheString = localStorage.getItem(key) as string;
        return JSON.parse(cacheString) as ICache;
    }

    protected override setCache<T>(key: string, data: T, expiresAfter: number) {
        const now = new Date().getTime();
        const cachedData: ICache = {
            data: data,
            expiresAt: new Date(now + expiresAfter).getTime()
        }
        localStorage.setItem(key, JSON.stringify(cachedData));
    }

    protected override clear(key: string) {
        const cache: ICache = { data: {}, expiresAt: 0 };
        localStorage.setItem(key, JSON.stringify(cache));
    }
}