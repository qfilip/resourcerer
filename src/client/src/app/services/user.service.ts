import { Injectable, inject, signal } from '@angular/core';
import { LocalstorageCacheService } from './localstorage.cache.service';
import { CacheFunctions } from '../models/services/ICache';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private _cache: CacheFunctions;

    constructor(private cacheService: LocalstorageCacheService) {
        const minutes = 30 * 60 * 1000;
        this._cache = this.cacheService.register('app-user', minutes);
    }

    
}