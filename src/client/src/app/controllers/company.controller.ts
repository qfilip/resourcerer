import { Injectable } from "@angular/core";
import { BaseController } from "./base.controller";
import { IV1CompanyOverview } from "../models/dtos/interfaces";
import { HttpParams } from "@angular/common/http";

@Injectable({
    providedIn: 'root'
})
export class CompanyController extends BaseController {
    private url = this.apiUrl + '/companies';

    getCompanyOverview(companyId: string) {
        const url = this.url + '/overview';
        return this.http.get<IV1CompanyOverview>(url, {
            params: new HttpParams().set('companyId', companyId)
        });
    }
}