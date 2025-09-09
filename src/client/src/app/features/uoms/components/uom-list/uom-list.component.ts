import { CommonModule } from '@angular/common';
import { Component, inject, output } from '@angular/core';
import { UomService } from '../../services/uom.service';
import { IUnitOfMeasureDto } from '../../../../shared/dtos/interfaces';
import { SearchListComponent } from "../../../../shared/features/common-ui/components/search-list/search-list.component";

@Component({
  standalone: true,
  selector: 'app-uom-list',
  imports: [CommonModule, SearchListComponent],
  templateUrl: './uom-list.component.html',
  styleUrl: './uom-list.component.css'
})
export class UomListComponent {
  private uomService = inject(UomService);
  $uoms = this.uomService.$uoms();
  $selectedUom = this.uomService.$selectedUom;

  displayFilter = (query: string) => (x: IUnitOfMeasureDto) => {
    return x.name.toLowerCase().includes(query);
  }

  onSelected = output<IUnitOfMeasureDto>();
}
