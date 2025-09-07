import { CommonModule } from '@angular/common';
import { Component, effect, inject, input, signal } from '@angular/core';
import { InstanceService } from '../../services/instance.service';
import { IItemDto, IV1InstanceInfo } from '../../../../shared/dtos/interfaces';

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
  $instancesInfo = this.service.$instancesInfo;
}
