import { Injectable, signal } from "@angular/core";
import { Observable, map, tap } from "rxjs";
import { IAppUserDto } from "../../../shared/dtos/interfaces";
import { CacheFunctions } from "../../../shared/services/cache/cache.models";
import { LocalstorageCacheService } from "../../../shared/services/cache/local-storage-cache.service";
import { jwtClaimKeys, permissionsMap } from "../../../shared/dtos/constants";
import { Result } from "../../../shared/models/result";
import { UserPermission } from "../models/userPermission";
import { UserApiService } from "./user.api.service";

@Injectable({
    providedIn: 'root',
})
export class UserService {
    private _cache: CacheFunctions<string>;
    private _user$ = signal<IAppUserDto | null>(null);
    private _jwt$ = signal<string | null>(null);

    user$ = this._user$.asReadonly();
    jwt$ = this._jwt$.asReadonly();

    constructor(
        private cacheService: LocalstorageCacheService,
        private userApiService: UserApiService
    ) {
        const minutes = 30 * 60 * 1000;
        this._cache = this.cacheService.register<string>(
            'app-user',
            () => {
                return new Observable<string>((sub) => {
                    sub.next('');
                    sub.complete();
                });
            },
            minutes
        );
    }

    login(x: IAppUserDto) {
        return this.userApiService.login(x)
            .pipe(
                tap(jwt => this.setUser(jwt))
            );
    }

    logout = () => this.clearUser();

    getFromCache() {
        const sub = this._cache.retrieve()
            .subscribe({
                next: jwt => {
                    if (!jwt) return;
                    this.setUser(jwt);
                    sub.unsubscribe();
                }
            })
    }

    setUser(jwt: string) {

        this._cache.setCache(jwt);
        const jwtData = this.parseJwt(jwt);
        
        this._user$.set(jwtData.dto);
        this._jwt$.set(jwt);
    }

    clearUser() {
        this._cache.clear();
        this._user$.set(null);
        this._jwt$.set(null);
    }

    private parseJwt(jwt: string) {
        if(jwt === '') {
            return {
                dto: {} as IAppUserDto,
                expired: true
            };
        }
    
        const body = jwt.split('.')[1];
        const jwtObj = JSON.parse(atob(body));
        
        const now = new Date().getTime() / 1000;
        const expiresAt = new Date(jwtObj['exp']).getTime();
        const expired = expiresAt <= now;
        
        const userDto = {
            id: jwtObj[jwtClaimKeys.id],
            name: jwtObj[jwtClaimKeys.name],
            displayName: jwtObj[jwtClaimKeys.displayName],
            email: jwtObj[jwtClaimKeys.email],
            isAdmin: jwtObj[jwtClaimKeys.isAdmin].toLowerCase() === 'true' ? true : false,
            company: {
                id: jwtObj[jwtClaimKeys.companyId],
                name: jwtObj[jwtClaimKeys.companyName]
            },
            permissionsMap: {}
        } as IAppUserDto;
    
        for(const key in permissionsMap) {
            const hasSection = jwtObj[key];
            
            if(hasSection) {
                userDto.permissionsMap[key] = jwtObj[key];
            }
        }
    
        return {
            dto: userDto,
            expired: expired
        };
    }
    
    private tryMapUser(name: string, email: string, isAdmin: boolean, permissions: UserPermission[]): Result<IAppUserDto> {
        const validate = (x: string, error: string) =>
            !x || x.length === 0 ? error : '';
    
        const errors = [
            validate(name, 'Name cannot be empty'),
            validate(email, 'Email cannot be empty'),
        ]
        .filter(x => x.length > 0);
    
        const result: Result<IAppUserDto> = {
            x: {
                name: name,
                email: email,
                isAdmin: isAdmin,
                permissionsMap: this.mapPermissions(permissions)
            } as IAppUserDto,
            errors: errors
        }
    
        return result;
    }
    
    private mapPermissions(xs: UserPermission[]) {
        const permissionMap: { [key: string]: string[] } = {};
        
        xs.forEach(x => {
            const ps = x.permissions
                .filter(p => p.hasPermission)
                .map(p => p.name);
    
            permissionMap[x.section] = ps;
        });
    
        return permissionMap;
    }
}