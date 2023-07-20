import { IAppUserDto } from "../interfaces/dtos/interfaces";
import * as userStore from "../services/user.store";

export function register(x: IAppUserDto) {

}

export function login(x: IAppUserDto, onLoginSuccess: () => void) {
    onLoginSuccess();
}

export function checkAuthStore(onAuthPresent: () => void) {
    if(userStore.checkUserLogged()) {
        onAuthPresent();
    }
}