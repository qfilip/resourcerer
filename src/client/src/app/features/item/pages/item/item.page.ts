import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ItemListComponent } from "../../components/item-list/item-list.component";
import { ItemService } from '../../services/item.service';
import { IItemDto } from '../../../../shared/dtos/interfaces';
import { DialogService } from '../../../../shared/features/common-ui/services/dialog.service';
import { PopupService } from '../../../../shared/features/common-ui/services/popup.service';
import { DialogOptions } from '../../../../shared/features/common-ui/models/dialog-options.model';
import { CreateElementItemFormComponent } from "../../components/create-element-item-form/create-element-item-form.component";
import { UpdateElementItemFormComponent } from "../../components/update-element-item-form/update-element-item-form.component";

@Component({
  standalone: true,
  selector: 'app-item',
  imports: [ItemListComponent, CreateElementItemFormComponent, UpdateElementItemFormComponent],
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

  onItemCreated(x: IItemDto) {
    this.popup.ok(`Item ${x.name} created`);
    this.$component.set(null);
  }

  onItemUpdated(x: IItemDto) {
    this.popup.ok(`Item ${x.name} updated`);
    this.$component.set(null);
  }

  onFormError(errors: string[]) {
    this.popup.pushMany(errors, 'warn', 'Invalid data');
    this.$component.set(null);
  }

  openDialog() {
    this.dialogService.open({
      header: 'Form selection',
      message: 'Select item type you wish to create',
      buttons: [
        { 
          label: 'Element',
          action: () => this.$component.set('ce')
        },
        { 
          label: 'Composite',
          action: () => this.dialogService.close()
        },
        { 
          label: 'Cancel',
          action: () => this.dialogService.close()
        }
      ]
    } as DialogOptions)
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
