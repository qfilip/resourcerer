import { Injectable, inject } from "@angular/core";
import { BaseController } from "./base.controller";
import { IAppUserDto } from "../models/dtos/interfaces";

@Injectable({
    providedIn: 'root'
})
export class UserController extends BaseController {
    private url = this.apiUrl + '/users';

    login(user: IAppUserDto) {
        const url = this.url + '/login';
        return this.http.post<string>(url, user);
    }
}