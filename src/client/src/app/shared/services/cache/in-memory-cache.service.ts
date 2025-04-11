import { Injectable } from "@angular/core";
import { Observable, iif, map, of, tap } from "rxjs";
import { CachedData } from "./cache.models";

@Injectable({
    providedIn: 'root'
})
export class InMemoryCacheService {
    private _cache = new Map<string, unknown>();
    private keys = {
        example: 'e.g.'
    };

    // inject service
    private exampleApiService = {
        getAll: () => of([1,2,3])
    }

    sampleData = {
        store: (xs: number[]) => this.store<number[]>(this.keys.example, xs),
        retrieve: () => this.retrieve<number[]>(this.keys.example, this.exampleApiService.getAll()),
        refresh: () =>
            this.exampleApiService.getAll()
                .pipe(tap(xs => this.store<number[]>(this.keys.example, xs)))
    }
    
    protected store<T>(key: string, data: T) {
        const cache: CachedData = { data: data, expiresAt: 0 }; 
        this._cache.set(key, cache);
    }

    protected retrieve<T>(key: string, source: Observable<T>): Observable<T> {
        const data = this._cache.get(key) as T;
        const cache = of(data);

        return iif(() => !data, source, cache)
            .pipe(tap((x) => this._cache.set(key, x)));
    }
}