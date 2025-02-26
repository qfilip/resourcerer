import { Component, EventEmitter, Input, OnInit, Output, Signal, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { IDialogOptions } from '../../../models/components/IDialogOptions';
import { IAppUserDto, IUnitOfMeasureDto } from '../../../models/dtos/interfaces';
import { DialogService } from '../../../services/dialog.service';

@Component({
  selector: 'uom-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './uom-list.component.html',
  styleUrl: './uom-list.component.css'
})
export class UomListComponent {
  @Input({ required: true }) appUser!: IAppUserDto;
  @Input({ required: true }) unitsOfMeasure$!: Signal<IUnitOfMeasureDto[] | null>;
  @Output() onEdit = new EventEmitter<IUnitOfMeasureDto>();
  @Output() onDelete = new EventEmitter<IUnitOfMeasureDto>();
  
  private dialogService = inject(DialogService);

  openDeleteDialog(uom: IUnitOfMeasureDto) {
    this.dialogService.open({
      header: 'Delete Unit of Measure',
      message: `Are you sure you wish to delete unit of measure: ${uom.name}?`,
      buttons: [
        {
          label: 'Yes',
          action: () => this.onDelete.emit(uom)
        },
        {
          label: 'Cancel'
        }
      ]
    } as IDialogOptions)
  }
}
