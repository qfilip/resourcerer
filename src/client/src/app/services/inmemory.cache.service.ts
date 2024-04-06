import { Injectable } from "@angular/core";
import { ICache } from "../models/services/ICache";
import { IAppUserDto } from "../models/dtos/interfaces";
import { Observable, iif, of, switchMap, tap } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class InMemoryCacheService {
    private _cache = new Map<string, unknown>();
    private keys = {
        companyUsers: 'companyUsers'
    };
    
    companyUsers = {
        store: (xs: IAppUserDto[]) => this.store<IAppUserDto[]>(this.keys.companyUsers, xs),
        retrieve: (source: Observable<IAppUserDto[]>) => this.retrieve<IAppUserDto[]>(this.keys.companyUsers, source),
        clear: () => this._cache.delete(this.keys.companyUsers)
    }
    
    protected store<T>(key: string, data: T) {
        const cache: ICache = { data: data, expiresAt: 0 }; 
        this._cache.set(key, cache);
    }

    protected retrieve<T>(key: string, source: Observable<T>): Observable<T> {
        const data = this._cache.get(key) as T;
        return iif(() => !!data, of(data), source);
    }
}