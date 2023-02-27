import type IUserLoginDto from '../interfaces/dtos/IUserLoginDto';
import * as loaderService from '../services/commonUi/loader.service';
import * as pb from '../services/pocketbase.service';

export function checkAuthStore(onValidCredentials: () => void) {
    if(pb.database.authStore.isValid) {
        onValidCredentials();
        return;
    }
}

export function login(dto: IUserLoginDto, callback: () => void) {
    loaderService.show('Logging in');
    
    if(dto.asAdmin) {
        pb.database.admins.authWithPassword(dto.email, dto.password)
            .then(x => {
                console.log(x)
                loaderService.hide();
                callback();
            })
            .catch(e => {
                loaderService.hide();
                console.log(e);
            })
    }
    else {
        console.log('regular user login not implemented');
    }
}