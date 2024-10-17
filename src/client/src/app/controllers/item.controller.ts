import { Injectable } from "@angular/core";
import { BaseController } from "./base.controller";
import { IItemDto, IV1CreateElementItem, IV1CreateElementItemFormDataDto, IV1EditElementItemFormData, IV1ItemShoppingDetails } from "../models/dtos/interfaces";
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

    getItemsShoppingDetails(companyId: string) {
        const url = this.url + '/shopping-list';
        return this.http.get<IV1ItemShoppingDetails[]>(url, {
            params: new HttpParams().set('companyId', companyId)
        });
    }

    getCreateElementItemFormData(companyId: string) {
        const url = this.url + '/create/element/form';
        return this.http.get<IV1CreateElementItemFormDataDto>(url, {
            params: new HttpParams().set('companyId', companyId)
        });
    }

    createElementItem(dto: IV1CreateElementItem) {
        const url = this.url + '/create/element';
        return this.http.post<string>(url, dto);
    }

    getEditElementItemFormData(itemId: string, companyId: string) {
        const url = this.url + '/edit/element/form';
        return this.http.get<IV1EditElementItemFormData>(url, {
            params: new HttpParams().set('itemId', itemId).set('companyId', companyId)
        });
    }
}