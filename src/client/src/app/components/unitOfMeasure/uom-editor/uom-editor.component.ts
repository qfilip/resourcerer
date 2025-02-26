import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { IPopup } from '../../../models/components/IPopup';
import { IUnitOfMeasureDto } from '../../../models/dtos/interfaces';
import { PopupService } from '../../../services/popup.service';

@Component({
  selector: 'uom-editor',
  standalone: true,
  imports: [],
  templateUrl: './uom-editor.component.html',
  styleUrl: './uom-editor.component.css'
})
export class UomEditorComponent {
  @Input() uom: IUnitOfMeasureDto = { name: '', symbol: '' } as IUnitOfMeasureDto;
  @Input({ required: true }) title!: 'Create' | 'Edit';
  @Output() onSubmitted = new EventEmitter<IUnitOfMeasureDto>();

  private popupService = inject(PopupService);

  onSubmit(ev: Event, name: string, symbol: string) {
    ev.preventDefault();

    const validate = (x: string, error: string) =>
      !x || x.length === 0 ? error : '';

    const errors = [
        validate(name, 'Name cannot be empty'),
        validate(symbol, 'Symbol cannot be empty'),
    ]
    .filter(x => x.length > 0)
    .map(x => ({ message: x, type: 'warning' } as IPopup));

    if(errors.length > 0) {
      this.popupService.many(errors);
      return;
    }

    this.onSubmitted.emit({...this.uom, name: name, symbol: symbol } as IUnitOfMeasureDto);
  }
}
