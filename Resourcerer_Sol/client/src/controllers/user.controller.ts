import type IUserLoginDto from '../interfaces/dtos/IUserLoginDto';
import type IUserResiterDto from '../interfaces/dtos/IUserRegisterDto';

export function register(x: IUserResiterDto) {

}

export function login(x: IUserLoginDto, onLoginSuccess: () => void) {
    onLoginSuccess();
}

export function checkAuthStore(onAuthPresent: () => void) {
    onAuthPresent();
}