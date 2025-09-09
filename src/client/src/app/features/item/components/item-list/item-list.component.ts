import { Component, inject, output } from '@angular/core';
import { ItemService } from '../../services/item.service';
import { IItemDto } from '../../../../shared/dtos/interfaces';
import { CommonModule } from '@angular/common';
import { SearchListComponent } from "../../../../shared/features/common-ui/components/search-list/search-list.component";

@Component({
  standalone: true,
  selector: 'app-item-list',
  imports: [CommonModule, SearchListComponent],
  templateUrl: './item-list.component.html',
  styleUrl: './item-list.component.css'
})
export class ItemListComponent {
  private itemService = inject(ItemService);
  $items = this.itemService.$items;
  
  $selectedItem = this.itemService.$selectedItem;
  displayFilter = (query: string) => (x: IItemDto) => {
    return x.name.toLowerCase().includes(query) ||
          x.id.toLowerCase().includes(query);
  }

  onSelected = output<IItemDto>();
  typeToken = {} as IItemDto;
}
