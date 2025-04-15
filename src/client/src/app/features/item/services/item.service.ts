import { inject, Injectable, signal } from "@angular/core";
import { ItemApiService } from "./item.api.service";
import { UserService } from "../../user/services/user.service";
import { IItemDto, IV1CreateElementItem } from "../../../shared/dtos/interfaces";
import { tap } from "rxjs";

@Injectable({ providedIn: 'root' })
export class ItemService {
  private apiService = inject(ItemApiService);
  private userService = inject(UserService);

  private _$items = signal<IItemDto[]>([]);
  private _$selectedItem = signal<IItemDto | null>(null);

  $items = this._$items.asReadonly();
  $selectedItem = this._$selectedItem.asReadonly();

  setSelectedItem = (x: IItemDto) => this._$selectedItem.set(x);

  getCompanyItems() {
    const user = this.userService.$user()!;

    this.apiService.getCompanyItems(user.company.id)
      .subscribe({
        next: xs => this._$items.set(xs)
      });
  }

  removeItem(x: IItemDto) { }

  // element item
  getElementItemFormData() {
    const user = this.userService.$user()!;
    return this.apiService.getElementItemFormData(user.company.id);
  }

  createElementItem(dto: IV1CreateElementItem) {
    return this.apiService.createElementItem(dto)
      .pipe(
        tap(x => {
          this._$items.update(xs => xs.concat(x));
          this._$selectedItem.set(null);
        })
      );
  }
}