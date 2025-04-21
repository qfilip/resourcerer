import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ItemListComponent } from "../../components/item-list/item-list.component";
import { ItemService } from '../../services/item.service';
import { IItemDto } from '../../../../shared/dtos/interfaces';
import { DialogService } from '../../../../shared/features/common-ui/services/dialog.service';
import { PopupService } from '../../../../shared/features/common-ui/services/popup.service';
import { DialogOptions } from '../../../../shared/features/common-ui/models/dialog-options.model';
import { CreateElementItemFormComponent } from "../../components/create-element-item-form/create-element-item-form.component";
import { UpdateElementItemFormComponent } from "../../components/update-element-item-form/update-element-item-form.component";
import { eItemType } from '../../../../shared/dtos/enums';
import { CreateCompositeItemFormComponent } from "../../components/create-composite-item-form/create-composite-item-form.component";
import { ItemProductionFormComponent } from '../../components/item-production-overview/item-production-form.component';

@Component({
  standalone: true,
  selector: 'app-item',
  imports: [ItemListComponent, CreateElementItemFormComponent, UpdateElementItemFormComponent, CreateCompositeItemFormComponent, ItemProductionFormComponent],
  templateUrl: './item.page.html',
  styleUrl: './item.page.css'
})
export class ItemPage implements OnInit {
  private popup = inject(PopupService);
  private dialogService = inject(DialogService);
  private itemService = inject(ItemService);
  
  $display = signal<ToDisplay | null>(null);
  $selectedItem = computed(() => this.itemService.$selectedItem());

  ngOnInit(): void {
    this.itemService.getCompanyItems();
  }

  selectItem = (x: IItemDto) => this.itemService.setSelectedItem(x);

  showForm(item?: IItemDto) {
    this.itemService.getItemType(item!)
      .subscribe({ next: v => {
        if(!item && v.data === eItemType.Element)
          this.$display.set('create-element-form');
        else if(!item && v.data === eItemType.Composite)
          this.$display.set('create-composite-form');
        else if(item && v.data === eItemType.Element)
          this.$display.set('update-element-form');
        else if(item && v.data === eItemType.Composite)
          this.$display.set('update-composite-form');
      }})
  }

  onItemCreated(x: IItemDto) {
    this.popup.ok(`Item ${x.name} created`);
    this.$display.set(null);
  }

  onItemUpdated(x: IItemDto) {
    this.popup.ok(`Item ${x.name} updated`);
    this.$display.set(null);
  }

  onFormError(errors: string[]) {
    this.popup.pushMany(errors, 'warn', 'Invalid data');
    this.$display.set(null);
  }

  openDialog() {
    this.dialogService.open({
      header: 'Form selection',
      message: 'Select item type you wish to create',
      buttons: [
        { 
          label: 'Element',
          action: () => this.$display.set('create-element-form')
        },
        { 
          label: 'Composite',
          action: () => this.$display.set('create-composite-form')
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

type ToDisplay = 
  | 'create-element-form'
  | 'update-element-form'
  | 'create-composite-form'
  | 'update-composite-form'
  | 'production-form';

