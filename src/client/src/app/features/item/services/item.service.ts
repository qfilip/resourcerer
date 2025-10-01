import { inject, Injectable, signal } from "@angular/core";
import { ItemApiService } from "./item.api.service";
import { UserService } from "../../user/services/user.service";
import { IItemDto, IV1CreateCompositeItem, IV1CreateElementItem, IV1CreateElementItemProductionOrderCommand, IV1UpdateElementItem } from "../../../shared/dtos/interfaces";
import { tap } from "rxjs";
import { Functions } from "../../../shared/utils/functions";

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
        tap(x => this.runReducer(x, 'create'))
      );
  }

  updateElementItem(dto: IV1UpdateElementItem) {
    return this.apiService.updateElementItem(dto)
      .pipe(
        tap(x => this.runReducer(x, 'update'))
      );
  }

  createCompositeItem(dto: IV1CreateCompositeItem) {
    return this.apiService.createCompositeItem(dto)
      .pipe(
        tap(x => this.runReducer(x, 'create'))
      );
  }

  removeItem(dto: IItemDto) {
    return this.apiService.removeItem(dto)
      .pipe(
        tap(x => this.runReducer(x, 'delete'))
      );
  }

  produceElement(cmd: IV1CreateElementItemProductionOrderCommand) {
    return this.apiService.produceElement(cmd);
  }

  private runReducer(x: IItemDto, action: 'create' | 'update' | 'delete') {
    const reducer = Functions.getReducer<IItemDto>(this._$items);
    reducer(x, action);
    this._$selectedItem.set(null);
  }
}