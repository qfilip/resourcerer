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
            console.log('no jwt');
            return false;
        }
        
        const jwtData = parseJwt(jwt);
        
        if(jwtData.expired) {
            console.log('expired');
            return false;
        }
        
        console.log('ok');
        this._user$.set(jwtData.dto);
        return true;
    }

    setUser(jwt: string) {
        this._cache.store(jwt);
        const jwtData = parseJwt(jwt)
        this._user$.set(jwtData.dto);
    }

    clearUser() {
        this._cache.clear();
        this._user$.set(null);
    }
}