import { Injectable, inject, signal } from '@angular/core';
import { LocalstorageCacheService } from './localstorage.cache.service';
import { CacheFunctions } from '../models/services/ICache';
import { IAppUserDto } from '../models/dtos/interfaces';
import { parseJwt } from '../functions/user.functions';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private _cache: CacheFunctions<string>;
    private _user$ = signal<IAppUserDto | null>(null);
    user = this._user$.asReadonly();

    constructor(private cacheService: LocalstorageCacheService) {
        const minutes = 30 * 60 * 1000;
        this._cache = this.cacheService.register<string>('app-user', minutes);
    }

    isLoggedIn() {
        const jwt = this._cache.retrieve();
        if(!jwt) {
            return false;
        }
        
        const jwtData = parseJwt(jwt);
        const now = new Date().getTime();
        const expired = jwtData.expiresAt <= now;

        if(expired) {
            return false;
        }
        
        this._user$.set(jwtData.dto);
        return true;
    }

    setUser(x: IAppUserDto) {
        this._cache.store(x.jwt);
        this._user$.set(x);
    }

    clearUser() {
        this._cache.clear();
        this._user$.set(null);
    }
}