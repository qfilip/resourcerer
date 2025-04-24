import { computed, inject, Injectable, signal } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { CategoryApiService } from "./category.api.service";
import { UserService } from "../../user/services/user.service";
import { ICategoryDto, IV1CreateCategory, IV1UpdateCategory } from "../../../shared/dtos/interfaces";
import { tap } from "rxjs";
import { CategoryUtils } from "../category.utils";
import { Functions } from "../../../shared/utils/functions";

@Injectable({ providedIn: 'root' })
export class CategoryService extends BaseApiService {
  private apiService = inject(CategoryApiService);
  private userService = inject(UserService);

  private _$categories = signal<ICategoryDto[]>([]);
  private _$selectedCategory = signal<ICategoryDto | null>(null);
  
  $categories = this._$categories.asReadonly();
  $categoryTrees = computed(() => CategoryUtils.mapTree(this._$categories()));
  $selectedCategory = this._$selectedCategory.asReadonly();

  private reducer = Functions.getReducer<ICategoryDto>(this._$categories, 'Category reducer failed');

  selectCategory = (x: ICategoryDto) => this._$selectedCategory.set(x);

  getAllCompanyCategories() {
    const user = this.userService.$user()!;
    
    this.apiService.getAllCompanyCategories(user.company.id)
      .subscribe({
        next: xs => this.runReducers(xs)
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
        tap(x => this.runReducers(undefined, x, undefined, undefined))
      );
  }

  updateCategory(dto: IV1UpdateCategory) {
    return this.apiService.updateCategory(dto)
      .pipe(
        tap(x => this.runReducers(undefined, undefined, x, undefined))
      );
  }

  removeCategory(dto: ICategoryDto) {
    return this.apiService.removeCategory(dto)
      .pipe(
        tap(x => this.runReducers(undefined, undefined, undefined, x))
      );
  }

  private runReducers(all?: ICategoryDto[], created?: ICategoryDto, updated?: ICategoryDto, removed?: ICategoryDto) {
    this.reducer(all, created, updated, removed);
    this._$selectedCategory.set(null);
  }
}