import type { IAppUserDto } from "../interfaces/dtos/interfaces";
import * as userStore from "../stores/user.store";
import { http, httpSilent } from './base.controller';


export function register(x: IAppUserDto, onRegisterSuccess: () => void) {
    http.post('/users/register', x)
    .then(jwt => {
        userStore.trySetUser(jwt.data);
        onRegisterSuccess();
    })
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