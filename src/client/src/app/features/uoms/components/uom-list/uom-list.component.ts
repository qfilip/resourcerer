import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, output, signal } from '@angular/core';
import { UomService } from '../../services/uom.service';
import { IUnitOfMeasureDto } from '../../../../shared/dtos/interfaces';

@Component({
  standalone: true,
  selector: 'app-uom-list',
  imports: [CommonModule],
  templateUrl: './uom-list.component.html',
  styleUrl: './uom-list.component.css'
})
export class UomListComponent {
  private uomService = inject(UomService);
  private $uoms = signal<IUnitOfMeasureDto[]>([]);
  private $query = signal<string>('');
  
  $selectedUom = this.uomService.$selectedUom;
  $displayedUoms = computed(() => {
    const uoms = this.$uoms();
    const query = this.$query();
    
    return uoms.filter(x => x.name.includes(query));
  });

  onCreate = output();
  onUpdate = output<IUnitOfMeasureDto>();
  onRemove = output<IUnitOfMeasureDto>();
  
  constructor() {
    effect(() => this.$uoms.set(this.uomService.$uoms()))
  }

  onQueryChanged = (query: string) => this.$query.set(query);
  setSelectedItem = (x: IUnitOfMeasureDto) => this.uomService.selectUom(x);
}
