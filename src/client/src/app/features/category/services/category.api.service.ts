import { Injectable } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { ICategoryDto, IV1CreateCategory } from "../../../shared/dtos/interfaces";
import { HttpParams } from "@angular/common/http";

@Injectable({ providedIn: 'root' })
export class CategoryApiService extends BaseApiService {
  private url = this.apiUrl + '/categories';

  getAllCompanyCategories(companyId: string) {
    return this.http.get<ICategoryDto[]>(this.url, {
      params: new HttpParams().set('companyId', companyId)
    }).pipe(this.withLoader());
  }

  createCategory(dto: IV1CreateCategory) {
    return this.http.post<ICategoryDto>(this.url, dto).pipe(this.withLoader());
  }
}