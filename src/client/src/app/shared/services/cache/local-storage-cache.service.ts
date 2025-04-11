import { Injectable } from "@angular/core";
import { CacheService } from "./cache.service";
import { Observable, of, tap } from "rxjs";
import { CachedData, CacheFunctions } from "./cache.models";

@Injectable({
    providedIn: 'root'
})
export class LocalstorageCacheService extends CacheService {
    private _cacheKeys = new Set<string>();
    
    hasKey = (key: string) => this._cacheKeys.has(key);

    public override register<T>(key: string, fetchFn: () => Observable<T>, expiresAfter?: number, unsafe?: boolean): CacheFunctions<T> {
        if(this._cacheKeys.has(key)) {
            if(!unsafe) {
                throw `Localstorage cache key ${key} already exists`
            }
        }

        this._cacheKeys.add(key);

        if(!localStorage.getItem(key)) {
            this.setCache<T>(key, null as T, 0);
        }

        return {
            retrieve: () => {
                return this.retrieve<T>(key, fetchFn, expiresAfter);
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

    protected retrieve<T>(key: string, fetchFn: () => Observable<T>, expiresAfter?: number): Observable<T> {
        const expired = this.hasExpired(key);

        return expired
          ? fetchFn()
              .pipe(
                tap(x => this.setCache<T>(key, x, expiresAfter))
            )
          : of(this.getCache(key).data as T);
    }

    protected override getCache(key: string): CachedData {
        const cacheString = localStorage.getItem(key) as string;
        return JSON.parse(cacheString) as CachedData;
    }

    protected override setCache<T>(key: string, data: T, expiresAfter?: number) {
        const now = new Date().getTime();
        const expiresAt = expiresAfter
            ? new Date(now + expiresAfter).getTime()
            : undefined;

        const cachedData: CachedData = {
            data: data,
            expiresAt: expiresAt
        }
        localStorage.setItem(key, JSON.stringify(cachedData));
    }

    protected override clear(key: string) {
        const cache: CachedData = { data: {}, expiresAt: 0 };
        localStorage.setItem(key, JSON.stringify(cache));
    }
}