import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ItemListComponent } from "../../components/item-list/item-list.component";
import { ItemService } from '../../services/item.service';
import { IItemDto } from '../../../../shared/dtos/interfaces';
import { DialogService } from '../../../../shared/features/common-ui/services/dialog.service';
import { PopupService } from '../../../../shared/features/common-ui/services/popup.service';
import { DialogOptions } from '../../../../shared/features/common-ui/models/dialog-options.model';

@Component({
  standalone: true,
  selector: 'app-item',
  imports: [ItemListComponent],
  templateUrl: './item.page.html',
  styleUrl: './item.page.css'
})
export class ItemPage implements OnInit {
  private popup = inject(PopupService);
  private dialogService = inject(DialogService);
  private itemService = inject(ItemService);
  
  $component = signal<'ce' | 'ue' | 'cc' | 'uc' | null>(null);
  $selectedItem = computed(() => this.itemService.$selectedItem());

  ngOnInit(): void {
    this.itemService.getCompanyItems();
  }

  showComponent(x: 'ce' | 'ue' | 'cc' | 'uc' | null) {
      this.$component.set(x);
  }

  openDialog() {
    this.dialogService.open({
      header: 'Form selection',
      message: 'Select item type you wish to create',
      buttons: [
        { 
          label: 'Element',
          action: () => this.showCreateElementItemForm()
        },
        { 
          label: 'Composite',
          action: () => this.showCreateCompositeItemForm()
        },
        { 
          label: 'Cancel',
          action: () => this.dialogService.close()
        }
      ]
    } as DialogOptions)
  }

  showCreateElementItemForm() {

  }

  showCreateCompositeItemForm() {

  }

  removeItem(dto: IItemDto) {
      this.dialogService.openCheck(
        `Remove category ${dto.name}?`,
        () =>
          this.itemService.removeItem(dto)
            // .subscribe({ next: () => this.popup.ok('Item removed')})
      );
  }
}
