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

@Component({
  standalone: true,
  selector: 'app-item',
  imports: [ItemListComponent, CreateElementItemFormComponent, UpdateElementItemFormComponent, CreateCompositeItemFormComponent],
  templateUrl: './item.page.html',
  styleUrl: './item.page.css'
})
export class ItemPage implements OnInit {
  private popup = inject(PopupService);
  private dialogService = inject(DialogService);
  private itemService = inject(ItemService);
  
  $formType = signal<'ce' | 'ue' | 'cc' | 'uc' | null>(null);
  $selectedItem = computed(() => this.itemService.$selectedItem());

  ngOnInit(): void {
    this.itemService.getCompanyItems();
  }

  showForm(item?: IItemDto) {
    this.itemService.getItemType(item!)
      .subscribe({ next: v => {
        if(!item && v.data === eItemType.Element)
          this.$formType.set('ce');
        else if(!item && v.data === eItemType.Composite)
          this.$formType.set('cc');
        else if(item && v.data === eItemType.Element)
          this.$formType.set('ue');
        else if(item && v.data === eItemType.Composite)
          this.$formType.set('uc');
      }})
  }

  onItemCreated(x: IItemDto) {
    this.popup.ok(`Item ${x.name} created`);
    this.$formType.set(null);
  }

  onItemUpdated(x: IItemDto) {
    this.popup.ok(`Item ${x.name} updated`);
    this.$formType.set(null);
  }

  onFormError(errors: string[]) {
    this.popup.pushMany(errors, 'warn', 'Invalid data');
    this.$formType.set(null);
  }

  openDialog() {
    this.dialogService.open({
      header: 'Form selection',
      message: 'Select item type you wish to create',
      buttons: [
        { 
          label: 'Element',
          action: () => this.$formType.set('ce')
        },
        { 
          label: 'Composite',
          action: () => this.$formType.set('cc')
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
