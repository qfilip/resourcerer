import { Component, EventEmitter, Input, Output, Signal, inject } from '@angular/core';
import { IDialogOptions } from '../../../models/components/IDialogOptions';
import { IAppUserDto, IItemDto } from '../../../models/dtos/interfaces';
import { DialogService } from '../../../services/dialog.service';

@Component({
  selector: 'item-list',
  standalone: true,
  imports: [],
  templateUrl: './item-list.component.html',
  styleUrl: './item-list.component.css'
})
export class ItemListComponent {
  @Input({ required: true }) appUser!: IAppUserDto;
  @Input({ required: true }) items$!: Signal<IItemDto[] | null>;
  
  @Output() onSelect = new EventEmitter<IItemDto>();
  @Output() onEdit = new EventEmitter<IItemDto>();
  @Output() onDelete = new EventEmitter<IItemDto>();

  private dialogService = inject(DialogService)

  openDeleteDialog(item: IItemDto) {
    this.dialogService.open({
      header: 'Delete Item',
      message: `Are you sure you wish to delete item: ${item.name}?`,
      buttons: [
        {
          label: 'Yes',
          action: () => this.onDelete.emit(item)
        },
        {
          label: 'Cancel'
        }
      ]
    } as IDialogOptions);
  }
}
