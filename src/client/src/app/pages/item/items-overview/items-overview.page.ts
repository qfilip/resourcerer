import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ItemListComponent } from '../components/item-list/item-list.component';
import { IItemDto } from '../../../models/dtos/interfaces';
import { UserService } from '../../../services/user.service';
import { ItemController } from '../../../controllers/item.controller';

@Component({
  selector: 'items-overview',
  standalone: true,
  imports: [CommonModule, ItemListComponent],
  templateUrl: './items-overview.page.html',
  styleUrl: './items-overview.page.css'
})
export class ItemsOverviewPage implements OnInit {
  private userService = inject(UserService);
  private itemController = inject(ItemController);

  appUser$ = computed(() => this.userService.user());
  items$ = signal<IItemDto[] | null>(null);

  ngOnInit() {
    const user = this.userService.user();
    if(!user) return;

    this.itemController
      .getCompanyItems(user.company.id)
      .subscribe({
        next: xs => this.items$.set(xs)
      });
  }
}
