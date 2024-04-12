import { Component, EventEmitter, Input, Output, Signal } from '@angular/core';
import { IAppUserDto, IItemDto } from '../../../../models/dtos/interfaces';

@Component({
  selector: 'app-item-list',
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

  openDeleteDialog(item: IItemDto) {

  }
}
