import { Injectable } from "@angular/core";
import { BaseController } from "./base.controller";
import { IAppUserDto, IV1EditUser, IV1RegisterUser } from "../models/dtos/interfaces";
import { HttpParams } from "@angular/common/http";
import { tap } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class UserController extends BaseController {
    private url = this.apiUrl + '/users';

    login(user: IAppUserDto) {
        const url = this.url + '/login';
        return this.http.post<string>(url, user);
    }

    getAllCompanyUsers(companyId: string) {
        const url = this.url + '/company-all';
        return this.http.get<IAppUserDto[]>(url, {
            params: new HttpParams().set('companyId', companyId)
        });
    }

    registerUser(dto: IV1RegisterUser) {
        const url = this.url + '/register-user';
        return this.http.post<IAppUserDto>(url, dto);
    }

    editUser(dto: IV1EditUser) {
        const url = this.url + '/edit';
        return this.http.post<IAppUserDto>(url, dto);
    }
}