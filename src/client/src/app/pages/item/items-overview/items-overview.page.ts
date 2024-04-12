import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ItemListComponent } from '../components/item-list/item-list.component';

@Component({
  selector: 'items-overview',
  standalone: true,
  imports: [CommonModule, ItemListComponent],
  templateUrl: './items-overview.page.html',
  styleUrl: './items-overview.page.css'
})
export class ItemsOverviewPage {

}
