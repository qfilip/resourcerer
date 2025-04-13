import { inject, Injectable, signal } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { CategoryApiService } from "./category.api.service";
import { UserService } from "../../user/services/user.service";
import { ICategoryDto, IV1CreateCategory } from "../../../shared/dtos/interfaces";
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
    dto.companyId = user.company.id;

    return this.apiService.createCategory(dto)
      .pipe(
        tap(x => this._$categories.update(xs => xs.concat(x))
      ));
  }
}