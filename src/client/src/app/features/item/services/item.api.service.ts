import { Injectable } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { IItemDto, IV1CreateElementItem, IV1ElementItemFormData } from "../../../shared/dtos/interfaces";
import { HttpParams } from "@angular/common/http";

@Injectable({ providedIn: 'root' })
export class ItemApiService extends BaseApiService {
  private url = this.apiUrl + '/items';

  getCompanyItems(companyId: string) {
    return this.http.get<IItemDto[]>(this.url, {
      params: new HttpParams().set('companyId', companyId)
    }).pipe(this.withLoader());
  }

  getElementItemFormData(companyId: string) {
    const url = this.url + '/element/formdata';
    return this.http.get<IV1ElementItemFormData>(url, {
      params: new HttpParams().set('companyId', companyId)
    }).pipe(this.withLoader());
  }

  createElementItem(dto: IV1CreateElementItem) {
    const url = this.url + '/create/element';
    return this.http.post<IItemDto>(url, dto).pipe(this.withLoader());
  }
}