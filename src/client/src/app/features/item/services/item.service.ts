import { inject, Injectable, signal } from "@angular/core";
import { ItemApiService } from "./item.api.service";
import { UserService } from "../../user/services/user.service";
import { IItemDto, IV1CreateCompositeItem, IV1CreateElementItem, IV1UpdateElementItem } from "../../../shared/dtos/interfaces";
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
        next: xs => this.runReducers(xs)
      });
  }

  removeItem(x: IItemDto) { }

  // element item
  getElementItemFormData() {
    const user = this.userService.$user()!;
    return this.apiService.getElementItemFormData(user.company.id);
  }

  getCompositeItemFormData() {
    const user = this.userService.$user()!;
    return this.apiService.getCompositeItemFormData(user.company.id);
  }

  getItemType(dto: IItemDto) {
    return this.apiService.getItemType(dto.id);
  }

  createElementItem(dto: IV1CreateElementItem) {
    return this.apiService.createElementItem(dto)
      .pipe(
        tap(x => {
          this.runReducers(undefined, x);
        })
      );
  }

  updateElementItem(dto: IV1UpdateElementItem) {
    return this.apiService.updateElementItem(dto)
      .pipe(
        tap(x => this.runReducers(undefined, undefined, x))
      );
  }

  createCompositeItem(dto: IV1CreateCompositeItem) {
    return this.apiService.createCompositeItem(dto)
      .pipe(
        tap(x => {
          this.runReducers(undefined, x);
        })
      );
  }

  private runReducers(all?: IItemDto[], created?: IItemDto, updated?: IItemDto, deleted?: IItemDto) {
      if(all)
        this._$items.set(all);
      else if(created)
        this._$items.update(xs => xs.concat(created));
      else if(updated)
        this._$items.update(xs => xs.filter(x => x.id !== updated.id).concat(updated));
      else if(deleted)
        this._$items.update(xs => xs.filter(x => x.id !== deleted.id));
      else
        throw 'Item reducer failed';
  
      this._$selectedItem.set(null);
    }
}