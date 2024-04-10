import { Injectable } from "@angular/core";
import { BaseController } from "./base.controller";
import { IUnitOfMeasureDto, IV1CompanyOverview } from "../models/dtos/interfaces";
import { HttpParams } from "@angular/common/http";

@Injectable({
    providedIn: 'root'
})
export class UnitOfMeasureController extends BaseController {
    private url = this.apiUrl + '/unitsOfMeasure';

    getCompanyUnitsOfMeasure(companyId: string) {
        const url = this.url + '/company-all';
        return this.http.get<IUnitOfMeasureDto[]>(url, {
            params: new HttpParams().set('companyId', companyId)
        });
    }
}