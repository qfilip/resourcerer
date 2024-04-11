import { Injectable } from "@angular/core";
import { BaseController } from "./base.controller";
import { IUnitOfMeasureDto, IV1CompanyOverview, IV1CreateUnitOfMeasure, IV1EditUnitOfMeasure } from "../models/dtos/interfaces";
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

    create(dto: IV1CreateUnitOfMeasure) {
        return this.http.post<IUnitOfMeasureDto>(this.url, dto);
    }

    edit(dto: IV1EditUnitOfMeasure) {
        const url = this.url + '/edit';
        return this.http.post<IUnitOfMeasureDto>(url, dto);
    }
}