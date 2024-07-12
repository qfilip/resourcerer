import { Injectable, inject, signal } from '@angular/core';
import { LocalstorageCacheService } from './localstorage.cache.service';
import { CacheFunctions } from '../models/services/ICache';
import { IAppUserDto } from '../models/dtos/interfaces';
import { parseJwt } from '../functions/user.functions';
import { map, Observable, tap } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    private _cache: CacheFunctions<string>;
    private _user$ = signal<IAppUserDto | null>(null);
    private _jwt$ = signal<string | null>(null);

    user = this._user$.asReadonly();
    jwt = this._jwt$.asReadonly();

    constructor(private cacheService: LocalstorageCacheService) {
        const minutes = 30 * 60 * 1000;
        this._cache = this.cacheService.register<string>(
            'app-user',
            () => {
                return new Observable<string>((sub) => {
                    sub.next(undefined);
                    sub.complete();
                });
            },
            minutes
        );
    }

    isLoggedIn() {
        return this._cache.retrieve().pipe(
            map((jwt) => {
                if (!jwt) {
                    return false;
                }

                const jwtData = parseJwt(jwt);

                if (jwtData.expired) {
                    return false;
                }

                return true;
            })
        );
    }

    setUser(jwt: string) {
        this._cache.setCache(jwt);
        const jwtData = parseJwt(jwt);

        this._user$.set(jwtData.dto);
        this._jwt$.set(jwt);
    }

    clearUser() {
        this._cache.clear();
        this._user$.set(null);
        this._jwt$.set(null);
    }
}
