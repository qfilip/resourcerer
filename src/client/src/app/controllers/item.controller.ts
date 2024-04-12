import { Injectable } from "@angular/core";
import { BaseController } from "./base.controller";
import { IItemDto } from "../models/dtos/interfaces";
import { HttpParams } from "@angular/common/http";

@Injectable({
    providedIn: 'root'
})
export class ItemController extends BaseController {
    private url = this.apiUrl + '/items';

    getCompanyItems(companyId: string) {
        const url = this.url + '/company-all';
        return this.http.get<IItemDto[]>(url, {
            params: new HttpParams().set('companyId', companyId)
        });
    }

    
}