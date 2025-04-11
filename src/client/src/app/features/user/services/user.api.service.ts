import { Injectable } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { HttpParams } from "@angular/common/http";
import { IAppUserDto, IV1RegisterUser, IV1EditUser } from "../../../shared/dtos/interfaces";

@Injectable({
    providedIn: 'root'
})
export class UserApiService extends BaseApiService {
    private url = this.apiUrl + '/users';

    login(user: IAppUserDto) {
        const url = this.url + '/login';
        return this.http.post<string>(url, user).pipe(this.withLoader());
    }

    getAllCompanyUsers(companyId: string) {
        const url = this.url + '/company-all';
        return this.http.get<IAppUserDto[]>(url, {
            params: new HttpParams().set('companyId', companyId)
        }).pipe(this.withLoader());
    }

    registerUser(dto: IV1RegisterUser) {
        const url = this.url + '/register-user';
        return this.http.post<IAppUserDto>(url, dto).pipe(this.withLoader());
    }

    editUser(dto: IV1EditUser) {
        const url = this.url + '/edit';
        return this.http.post<IAppUserDto>(url, dto).pipe(this.withLoader());
    }
}