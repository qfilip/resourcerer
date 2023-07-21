import type { IAppUserDto } from "../interfaces/dtos/interfaces";
import * as userStore from "../services/user.store";
import { http } from './base.controller';


export function register(x: IAppUserDto) {

}

export function login(x: IAppUserDto, onLoginSuccess: () => void) {
    http.post('/users/login', x)
    .then(jwt => {
        userStore.setUser(jwt.data);
        onLoginSuccess();
    })
    .catch(err => console.warn(err));
}

export function checkAuthStore(onAuthPresent: () => void) {
    if(userStore.checkUserLogged()) {
        onAuthPresent();
    }
}