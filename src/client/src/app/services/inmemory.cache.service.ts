import { Injectable } from "@angular/core";
import { CacheService } from "./cache.service";
import { CacheFunctions, ICache } from "../models/services/ICache";

@Injectable({
    providedIn: 'root'
})
export class InMemoryCacheService extends CacheService {
    private _cache = new Map<string, unknown>();

    public override register<T>(key: string, expiresAfter: number): CacheFunctions<T> {
        if(this._cache.has(key)) {
            throw `Memory cache key ${key} already exists`;
        }

        const cache: ICache = { data: {} as T, expiresAt: 0 };
        this._cache.set(key, cache);

        return {
            store: (x: T) => {
                this.store(key, x, 0);
            },
            retrieve: () => {
                return this.retrieve<T>(key);
            },
            clear: () => {
                this.clear(key);
            }
        }
    }
    protected override store<T>(key: string, data: T, _: number): void {
        const cache: ICache = { data: data, expiresAt: 0 }; 
        this._cache.set(key, cache);
    }
    protected override retrieve<T>(key: string): T | null {
        return this._cache.get(key) as T;
    }

    protected override clear(key: string) {
        this._cache.delete(key);
    }

}