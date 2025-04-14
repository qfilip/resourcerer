import { Injectable } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { IItemDto, IV1CreateElementItem, IV1CreateElementItemFormDataDto, IV1EditElementItemFormData } from "../../../shared/dtos/interfaces";
import { HttpParams } from "@angular/common/http";

@Injectable({ providedIn: 'root' })
export class ItemApiService extends BaseApiService {
  private url = this.apiUrl + '/items';

  getCompanyItems(companyId: string) {
    return this.http.get<IItemDto[]>(this.url, {
      params: new HttpParams().set('companyId', companyId)
    }).pipe(this.withLoader());
  }

  getCreateElementItemFormData(companyId: string) {
    const url = this.url + '/create/element/form';
    return this.http.get<IV1CreateElementItemFormDataDto>(url, {
      params: new HttpParams().set('companyId', companyId)
    }).pipe(this.withLoader());
  }

  createElementItem(dto: IV1CreateElementItem) {
    const url = this.url + '/create/element';
    return this.http.post<IItemDto>(url, dto).pipe(this.withLoader());
  }

  getEditElementItemFormData(itemId: string, companyId: string) {
    const url = this.url + '/edit/element/form';
    return this.http.get<IV1EditElementItemFormData>(url, {
      params: new HttpParams().set('itemId', itemId).set('companyId', companyId)
    }).pipe(this.withLoader());
  }
}