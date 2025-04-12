import { inject, Injectable, signal } from "@angular/core";
import { BaseApiService } from "../../../shared/services/base-api.service";
import { CategoryApiService } from "./category.api.service";
import { UserService } from "../../user/services/user.service";
import { ICategoryDto } from "../../../shared/dtos/interfaces";

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
}