import { Component, computed, inject, input, output } from '@angular/core';
import { IUnitOfMeasureDto, IV1CreateUnitOfMeasure, IV1EditUnitOfMeasure } from '../../../../shared/dtos/interfaces';
import { FormObject, FormObjectControl } from '../../../../shared/utils/forms';
import { Validation } from '../../../../shared/utils/validation';
import { UomService } from '../../services/uom.service';
import { FormErrorComponent } from "../../../../shared/features/common-ui/components/form-error/form-error.component";

@Component({
  standalone: true,
  selector: 'app-update-uom-form',
  imports: [FormErrorComponent],
  templateUrl: './update-uom-form.component.html',
  styleUrl: './update-uom-form.component.css'
})
export class UpdateUomFormComponent {
  private uomService = inject(UomService);
  onSubmitted = output();
  
  $uom = input.required<IUnitOfMeasureDto>();
  $form = computed(() => {
    const uom = this.$uom();
    
    return new FormObject({
      name: new FormObjectControl<string>({
        value: uom.name,
        validators: [
          { fn: Validation.notNull, error: 'Name is required' },
          { fn: Validation.minLength(2), error: 'Name must be at least 2 characters long' }
        ]
      }),
      symbol: new FormObjectControl<string>({
        value: uom.symbol,
        validators: [
          { fn: Validation.notNull, error: 'Symbol is required' },
          { fn: Validation.minLength(2), error: 'Name must be at least 2 characters long' }
        ]
      })
    });
  });

  onSubmit(ev: Event) {
    ev.preventDefault();
    const form = this.$form()!;
    if (!form.valid) return;

    const dto = {
      id: this.$uom().id,
      name: form.controls.name.data.value!,
      symbol: form.controls.symbol.data.value!,
    } as IV1EditUnitOfMeasure;

    this.uomService.editUnitOfMeasure(dto)
      .subscribe({
        next: () => this.onSubmitted.emit()
      });
  }
}
