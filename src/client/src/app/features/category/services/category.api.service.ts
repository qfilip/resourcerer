import { Injectable } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { ICategoryDto, IV1CreateCategory, IV1UpdateCategory } from "../../../shared/dtos/interfaces";
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

  updateCategory(dto: IV1UpdateCategory) {
    const url = this.url + '/update';
    return this.http.post<ICategoryDto>(url, dto).pipe(this.withLoader());
  }

  removeCategory(dto: ICategoryDto) {
    return this.http.delete<string>(this.url, { body: dto }).pipe(this.withLoader());
  }
}