import { Injectable } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { IUnitOfMeasureDto, IV1CreateUnitOfMeasure, IV1EditUnitOfMeasure } from "../../../shared/dtos/interfaces";
import { HttpParams } from "@angular/common/http";

@Injectable({ providedIn: 'root'})
export class UomApiService extends BaseApiService {
  private url = this.apiUrl + '/unitsOfMeasure';

  getCompanyUnitsOfMeasure(companyId: string) {
    return this.http.get<IUnitOfMeasureDto[]>(this.url, {
      params: new HttpParams().set('companyId', companyId)
    }).pipe(this.withLoader());
  }

  createUnitOfMeasure(dto: IV1CreateUnitOfMeasure) {
    return this.http.post<IUnitOfMeasureDto>(this.url, dto).pipe(this.withLoader());
  }

  editUnitOfMeasure(dto: IV1EditUnitOfMeasure) {
    const url = this.url + '/edit';
    return this.http.post<IUnitOfMeasureDto>(url, dto).pipe(this.withLoader());
  }

  deleteUnitOfMeasure(id: string) {
    return this.http.delete<IUnitOfMeasureDto>(this.url, {
      params: new HttpParams().set('id', id)
    }).pipe(this.withLoader());
  }
}