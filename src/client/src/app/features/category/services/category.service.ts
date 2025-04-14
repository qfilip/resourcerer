import { inject, Injectable, signal } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { CategoryApiService } from "./category.api.service";
import { UserService } from "../../user/services/user.service";
import { ICategoryDto, IV1CreateCategory, IV1UpdateCategory } from "../../../shared/dtos/interfaces";
import { tap } from "rxjs";
import { CategoryUtils } from "../category.utils";

@Injectable({ providedIn: 'root' })
export class CategoryService extends BaseApiService {
  private apiService = inject(CategoryApiService);
  private userService = inject(UserService);

  private _$categories = signal<ICategoryDto[]>([]);
  private _$categoryTrees = signal<ICategoryDto[]>([]);
  private _$selectedCategory = signal<ICategoryDto | null>(null);
  
  $categories = this._$categories.asReadonly();
  $categoryTrees = this._$categoryTrees.asReadonly();
  $selectedCategory = this._$selectedCategory.asReadonly();

  selectCategory = (x: ICategoryDto) => this._$selectedCategory.set(x);

  getAllCompanyCategories() {
    const user = this.userService.$user()!;
    
    this.apiService.getAllCompanyCategories(user.company.id)
      .subscribe({
        next: xs => this.runReducers(CategoryUtils.flattenTree(xs))
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
        tap(x => {
          const xs = this._$categories().concat(x);
          this.runReducers(xs);
        })
      );
  }

  updateCategory(dto: IV1UpdateCategory) {
    return this.apiService.updateCategory(dto)
      .pipe(
        tap(x => {
          const xs = this._$categories().filter(c => c.id !== x.id);
          this.runReducers(xs.concat(x));
        })
      );
  }

  removeCategory(dto: ICategoryDto) {
    return this.apiService.removeCategory(dto)
      .pipe(
        tap(id => {
          const xs = this._$categories().filter(c => c.id !== id);
          this.runReducers(xs);
        })
      );
  }

  private runReducers(xs: ICategoryDto[]) {
    this._$categories.set(xs);
    this._$categoryTrees.set(CategoryUtils.mapTree(xs));
    this._$selectedCategory.set(null);
  }
}