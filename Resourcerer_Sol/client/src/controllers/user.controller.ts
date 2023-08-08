import type { IAppUserDto, ISetUserPermissionsDto } from "../interfaces/dtos/interfaces";
import * as userStore from "../stores/user.store";
import { http, httpSilent } from './base.controller';

export function getAll() {
    return http.get('/users/all')
        .then(xs => xs.data as IAppUserDto[])
        .catch(err => console.warn(err));
}

export function getUser(userId: string) {
    return http.get('/users/', { params: { userId: userId } })
        .then(xs => xs.data as IAppUserDto)
        .catch(err => console.warn(err));
}

export function register(x: IAppUserDto, onRegisterSuccess: () => void) {
    http.post('/users/register', x)
    .then(jwt => {
        userStore.trySetUser(jwt.data);
        onRegisterSuccess();
    })
    .catch(err => console.warn(err));
}

export function setPermissions(x: ISetUserPermissionsDto) {
    return http.post('/users/set-permissions', x)
        .then(x => x.data as IAppUserDto)
        .catch(err => console.warn(err));
}

export function login(x: IAppUserDto, onLoginSuccess: () => void) {
    http.post('/users/login', x)
    .then(jwt => {
        userStore.trySetUser(jwt.data);
        onLoginSuccess();
    })
    .catch(err => console.warn(err));
}

export function refreshSession() {
    httpSilent.get('/users/refresh-session')
    .then(jwt => {
        userStore.trySetUser(jwt.data);
    })
    .catch(err => console.warn(err));
}