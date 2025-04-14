import { Component, inject, output } from '@angular/core';
import { UomService } from '../../services/uom.service';
import { IV1CreateUnitOfMeasure } from '../../../../shared/dtos/interfaces';
import { FormObject, FormObjectControl } from '../../../../shared/utils/forms';
import { Validation } from '../../../../shared/utils/validation';

@Component({
  selector: 'app-create-uom-form',
  imports: [],
  templateUrl: './create-uom-form.component.html',
  styleUrl: './create-uom-form.component.css'
})
export class CreateUomFormComponent {
  private uomService = inject(UomService);

  onCreated = output();

  f = new FormObject({
    name: new FormObjectControl<string>({
      value: '',
      validators: [
        { fn: Validation.notNull, error: 'Name is required' },
        { fn: Validation.minLength(2), error: 'Name must be at least 2 characters long' }
      ]
    }),
    symbol: new FormObjectControl<string>({
      value: '',
      validators: [
        { fn: Validation.notNull, error: 'Symbol is required' },
        { fn: Validation.minLength(2), error: 'Name must be at least 2 characters long' }
      ]
    })
  });

  onSubmit(ev: Event) {
    ev.preventDefault();
    if (!this.f.valid) {
      return;
    }

    const dto = {
      name: this.f.controls.name.data.value!,
      symbol: this.f.controls.symbol.data.value!,
    } as IV1CreateUnitOfMeasure;

    this.uomService.createUnitOfMeasure(dto)
      .subscribe({
        next: () => this.onCreated.emit()
      });
  }
}
