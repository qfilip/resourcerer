import { Component, computed, inject, signal } from '@angular/core';
import { UomListComponent } from "../../components/uom-list/uom-list.component";
import { IUnitOfMeasureDto } from '../../../../shared/dtos/interfaces';
import { UomService } from '../../services/uom.service';
import { DialogService } from '../../../../shared/features/common-ui/services/dialog.service';
import { PopupService } from '../../../../shared/features/common-ui/services/popup.service';
import { CreateUomFormComponent } from "../../components/create-uom-form/create-uom-form.component";
import { UpdateUomFormComponent } from "../../components/update-uom-form/update-uom-form.component";

@Component({
  standalone: true,
  selector: 'app-uoms',
  imports: [UomListComponent, CreateUomFormComponent, UpdateUomFormComponent],
  templateUrl: './uoms.page.html',
  styleUrl: './uoms.page.css'
})
export class UomsPage {
  private popup = inject(PopupService);
  private dialogService = inject(DialogService);
  private uomService = inject(UomService);
  
  $component = signal<'createForm' | 'updateForm' | null>(null);
  $selectedUom = this.uomService.$selectedUom;

  ngOnInit(): void {
    this.uomService.getCompanyUnitsOfMeasure();
  }

  showComponent(x: 'createForm' | 'updateForm' | null) {
    this.$component.set(x);
  }

  deleteUom(dto: IUnitOfMeasureDto) {
      this.dialogService.openCheck(
        `Delete unit of measure ${dto.name}?`,
        () =>
          this.uomService.deleteUnitOfMeasure(dto)
            .subscribe({ next: () => this.popup.ok('Unit of measure removed')})
      );
  }
}
