import { Injectable } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { IInstanceDto } from "../../../shared/dtos/interfaces";
import { HttpParams } from "@angular/common/http";

@Injectable({ providedIn: 'root' })
export class InstanceApiService extends BaseApiService{
  private url = this.apiUrl + '/instances';

  getItemInstances(itemId: string) {
    return this.http.get<IInstanceDto[]>(this.url, {
      params: new HttpParams().set('itemId', itemId)
    }).pipe(this.withLoader());
  }
}