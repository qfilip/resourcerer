import { Injectable } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { IItemDto } from "../../../shared/dtos/interfaces";
import { HttpParams } from "@angular/common/http";

@Injectable({ providedIn: 'root' })
export class ItemApiService extends BaseApiService {
    private url = this.apiUrl + '/items';

    getCompanyItems(companyId: string) {
        return this.http.get<IItemDto[]>(this.url, {
            params: new HttpParams().set('companyId', companyId)
        }).pipe(this.withLoader());
    }
}