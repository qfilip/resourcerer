import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { IAppUserDto, IUnitOfMeasureDto } from '../../../../models/dtos/interfaces';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { DialogService } from '../../../../services/dialog.service';
import { IDialogOptions } from '../../../../models/components/IDialogOptions';
import { UnitOfMeasureController } from '../../../../controllers/unitOfMeasure.controller';

@Component({
  selector: 'uom-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './uom-list.component.html',
  styleUrl: './uom-list.component.css'
})
export class UomListComponent implements OnInit {
  @Input({ required: true }) appUser!: IAppUserDto;
  @Output() onEdit = new EventEmitter<IUnitOfMeasureDto>();
  
  private dialogService = inject(DialogService);
  private uomController = inject(UnitOfMeasureController);
  
  unitsOfMeasure$: Observable<IUnitOfMeasureDto[]> | null = null;

  ngOnInit() {
    this.unitsOfMeasure$ = this.uomController
      .getCompanyUnitsOfMeasure(this.appUser.company.id);
  }

  openDeleteDialog(uom: IUnitOfMeasureDto) {
    this.dialogService.open({
      header: 'Delete Unit of Measure',
      message: `Are you sure you wish to delete unit of measure: ${uom.name}?`,
      buttons: [
        {
          label: 'Yes',
          action: () => {}
        },
        {
          label: 'Cancel',
          action: () => this.dialogService.close()
        }
      ]
    } as IDialogOptions)
  }
}
