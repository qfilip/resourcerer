import { Injectable } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { IV1InstanceInfo } from "../../../shared/dtos/interfaces";
import { HttpParams } from "@angular/common/http";

@Injectable({ providedIn: 'root' })
export class InstanceApiService extends BaseApiService{
  private url = this.apiUrl + '/instances';

  getItemInstancesInfo(itemId: string) {
    const url = this.url + '/info';
    return this.http.get<IV1InstanceInfo[]>(url, {
      params: new HttpParams().set('itemId', itemId)
    }).pipe(this.withLoader());
  }
}