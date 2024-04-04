import { Injectable, inject, signal } from '@angular/core';
import { LocalstorageCacheService } from './localstorage.cache.service';
import { CacheFunctions } from '../models/services/ICache';
import { IAppUserDto } from '../models/dtos/interfaces';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private _cache: CacheFunctions;
    private _user$ = signal<IAppUserDto | null>(null);
    user = this._user$.asReadonly();

    constructor(private cacheService: LocalstorageCacheService) {
        const minutes = 30 * 60 * 1000;
        this._cache = this.cacheService.register('app-user', minutes);
    }

    isLoggedIn() {
        let userDto = this._cache.retrieve<IAppUserDto>();
        this._user$.set(userDto);
        
        return userDto ? true : false;
    }

    setUser(x: IAppUserDto) {
        this._cache.store(x);
        this._user$.set(x);
    }

    clearUser() {
        this._cache.clear();
        this._user$.set(null);
    }
}