import { Injectable, inject } from "@angular/core";
import { ICache } from "../models/services/ICache";
import { IAppUserDto } from "../models/dtos/interfaces";
import { Observable, iif, of, tap } from "rxjs";
import { UserController } from "../controllers/user.controller";

@Injectable({
    providedIn: 'root'
})
export class InMemoryCacheService {
    private userController = inject(UserController);
    private _cache = new Map<string, unknown>();
    private keys = {
        companyUsers: 'companyUsers'
    };
    
    companyUsers = {
        store: (xs: IAppUserDto[]) => this.store<IAppUserDto[]>(this.keys.companyUsers, xs),
        retrieve: (companyId: string) => this.retrieve<IAppUserDto[]>(this.keys.companyUsers, this.userController.getAllCompanyUsers(companyId)),
        clear: () => this._cache.delete(this.keys.companyUsers)
    }
    
    protected store<T>(key: string, data: T) {
        const cache: ICache = { data: data, expiresAt: 0 }; 
        this._cache.set(key, cache);
    }

    protected retrieve<T>(key: string, source: Observable<T>): Observable<T> {
        const data = this._cache.get(key) as T;
        const cache = of(data);

        return iif(() => !data, source, cache)
            .pipe(tap((x) => this._cache.set(key, x)));
    }
}