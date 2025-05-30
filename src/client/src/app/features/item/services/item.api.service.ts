import { Injectable } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { IItemDto, ISimple, IV1CompositeItemFormData, IV1CreateCompositeItem, IV1CreateElementItem, IV1CreateElementItemProductionOrderCommand, IV1ElementItemFormData, IV1UpdateElementItem } from "../../../shared/dtos/interfaces";
import { HttpParams } from "@angular/common/http";
import { eItemType } from "../../../shared/dtos/enums";

@Injectable({ providedIn: 'root' })
export class ItemApiService extends BaseApiService {
  private url = this.apiUrl + '/items';

  getCompanyItems(companyId: string) {
    return this.http.get<IItemDto[]>(this.url, {
      params: new HttpParams().set('companyId', companyId)
    }).pipe(this.withLoader());
  }

  getItemType(itemId: string) {
    const url = this.url + '/type';
    return this.http.get<ISimple<eItemType>>(url, {
      params: new HttpParams().set('itemId', itemId)
    }).pipe(this.withLoader());
  }

  getElementItemFormData(companyId: string) {
    const url = this.url + '/element/formdata';
    return this.http.get<IV1ElementItemFormData>(url, {
      params: new HttpParams().set('companyId', companyId)
    }).pipe(this.withLoader());
  }

  getCompositeItemFormData(companyId: string) {
    const url = this.url + '/composite/formdata';
    return this.http.get<IV1CompositeItemFormData>(url, {
      params: new HttpParams().set('companyId', companyId)
    }).pipe(this.withLoader());
  }

  createElementItem(dto: IV1CreateElementItem) {
    const url = this.url + '/create/element';
    return this.http.post<IItemDto>(url, dto).pipe(this.withLoader());
  }

  updateElementItem(dto: IV1UpdateElementItem) {
    const url = this.url + '/update/element';
    return this.http.post<IItemDto>(url, dto).pipe(this.withLoader());
  }

  createCompositeItem(dto: IV1CreateCompositeItem) {
    const url = this.url + '/create/composite';
    return this.http.post<IItemDto>(url, dto).pipe(this.withLoader());
  }

  removeItem(dto: IItemDto) {
    return this.http.delete<IItemDto>(this.url, {
      params: new HttpParams().set('itemId', dto.id)
    }).pipe(this.withLoader());
  }

  // production
  produceElement(cmd: IV1CreateElementItemProductionOrderCommand) {
    const url = this.url + '/element/production/create';
    return this.http.post<unknown>(url, cmd).pipe(this.withLoader(true));
  }
}