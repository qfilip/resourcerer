import { inject, Injectable, signal } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { CategoryApiService } from "./category.api.service";
import { UserService } from "../../user/services/user.service";
import { ICategoryDto, IV1CreateCategory, IV1UpdateCategory } from "../../../shared/dtos/interfaces";
import { tap } from "rxjs";

@Injectable({ providedIn: 'root' })
export class CategoryService extends BaseApiService {
  private apiService = inject(CategoryApiService);
  private userService = inject(UserService);

  private _$categories = signal<ICategoryDto[]>([]);
  $categories = this._$categories.asReadonly();

  getAllCompanyCategories() {
    const user = this.userService.$user()!;
    
    this.apiService.getAllCompanyCategories(user.company.id)
      .subscribe({
        next: xs => this._$categories.set(xs)
      });
  }

  createCategory(dto: IV1CreateCategory) {
    const user = this.userService.$user()!;
    const request: IV1CreateCategory = {
      ...dto,
      companyId: user.company.id
    };

    return this.apiService.createCategory(request)
      .pipe(
        tap(x => this._$categories.update(xs => xs.concat(x))
      ));
  }

  updateCategory(id: string, newName: string, newParentCategoryId?: string) {
    const dto: IV1UpdateCategory = {
      categoryId: id,
      newName: newName,
      newParentCategoryId: newParentCategoryId
    };

    return this.apiService.updateCategory(dto)
      .pipe(
        tap(x => this._$categories.update(xs => {
          xs = xs.filter(c => c.id !== x.id);
          return xs.concat(x);
        })
      ));
  }
}