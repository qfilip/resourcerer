import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, output, signal } from '@angular/core';
import { InstanceService } from '../../services/instance.service';
import { IInstanceDto } from '../../../../shared/dtos/interfaces';

@Component({
  standalone: true,
  selector: 'app-item-instance-list',
  imports: [CommonModule],
  templateUrl: './item-instance-list.component.html',
  styleUrl: './item-instance-list.component.css'
})
export class ItemInstanceListComponent {
  private service = inject(InstanceService);

  $instances = signal<IInstanceDto[]>([]);
  $selected = this.service.$selectedInstance;

  onSelected = output<IInstanceDto>();
  
  constructor() {
    effect(() => this.$instances.set(this.service.$instances()))
  }
}
