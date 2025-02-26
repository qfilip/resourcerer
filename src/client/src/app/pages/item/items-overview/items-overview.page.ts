import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { IItemDto } from '../../../models/dtos/interfaces';
import { UserService } from '../../../services/user.service';
import { ItemController } from '../../../controllers/item.controller';
import { CreateItemComponent } from '../../../components/item/item-form/item-form.component';
import { ItemListComponent } from '../../../components/item/item-list/item-list.component';

@Component({
    selector: 'items-overview',
    standalone: true,
    templateUrl: './items-overview.page.html',
    styleUrl: './items-overview.page.css',
    imports: [CommonModule, ItemListComponent, CreateItemComponent]
})
export class ItemsOverviewPage implements OnInit {
  private userService = inject(UserService);
  private itemController = inject(ItemController);

  appUser$ = computed(() => this.userService.user());
  items$ = signal<IItemDto[] | null>(null);
  component$ = signal<null | 'create' | 'edit'>(null);

  ngOnInit() {
    this.loadItems();
  }

  setComponent(x: null | 'create' | 'edit') {
    this.component$.set(x);
  }

  onSelected(item: IItemDto) {
    console.log(item);
  }

  onItemCreated() {
    this.setComponent(null);
    this.loadItems();
  }

  private loadItems() {
    const user = this.userService.user();
    if(!user) return;

    this.itemController
      .getCompanyItems(user.company.id)
      .subscribe({
        next: xs => this.items$.set(xs)
      });
  }
}
