import { inject, Injectable, signal } from "@angular/core";
import { ItemApiService } from "./item.api.service";
import { UserService } from "../../user/services/user.service";
import { IItemDto } from "../../../shared/dtos/interfaces";

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

    removeItem(x: IItemDto) {}
}