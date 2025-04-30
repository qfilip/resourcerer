import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { InstanceService } from '../../services/instance.service';
import { IInstanceDto, IItemDto, IV1InstanceInfo } from '../../../../shared/dtos/interfaces';
import { ItemService } from '../../../item/services/item.service';

@Component({
  standalone: true,
  selector: 'app-item-instance-list',
  imports: [CommonModule],
  templateUrl: './item-instance-list.component.html',
  styleUrl: './item-instance-list.component.css'
})
export class ItemInstanceListComponent {
  private service = inject(InstanceService);

  $item = input.required<IItemDto>();
  $instancesInfo = signal<IV1InstanceInfo[]>([]);
  
  constructor() {
    effect(() => this.$instancesInfo.set(this.service.$instancesInfo()))
  }
}
