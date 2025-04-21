import { Component, computed, effect, inject, output, signal } from '@angular/core';
import { ItemService } from '../../services/item.service';
import { IItemDto } from '../../../../shared/dtos/interfaces';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-item-list',
  imports: [CommonModule],
  templateUrl: './item-list.component.html',
  styleUrl: './item-list.component.css'
})
export class ItemListComponent {
  private itemService = inject(ItemService);
  private $items = signal<IItemDto[]>([]);
  private $query = signal<string>('');
  
  $selectedItem = this.itemService.$selectedItem;
  $displayedItems = computed(() => {
    const items = this.$items();
    const query = this.$query().toLowerCase();
    
    return items.filter(x => 
      x.name.toLowerCase().includes(query) ||
      x.id.toLowerCase().includes(query));
  });

  onCreate = output();
  onSelected = output<IItemDto>();
  onProduce = output<IItemDto>();
  onRemove = output<IItemDto>();
  
  constructor() {
    effect(() => this.$items.set(this.itemService.$items()))
  }

  onQueryChanged = (query: string) => this.$query.set(query);
}
