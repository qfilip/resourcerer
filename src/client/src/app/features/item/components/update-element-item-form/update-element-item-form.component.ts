import { Component, computed, inject, input, OnInit, output, signal } from '@angular/core';
import { IItemDto, IV1ElementItemFormData, IV1UpdateElementItem } from '../../../../shared/dtos/interfaces';
import { ItemService } from '../../services/item.service';
import { FormObject, FormObjectControl } from '../../../../shared/utils/forms';
import { Validation } from '../../../../shared/utils/validation';
import { FormErrorComponent } from "../../../../shared/features/common-ui/components/form-error/form-error.component";
import { fromPromise } from 'rxjs/internal/observable/innerFrom';

@Component({
  standalone: true,
  selector: 'app-update-element-item-form',
  imports: [FormErrorComponent],
  templateUrl: './update-element-item-form.component.html',
  styleUrl: './update-element-item-form.component.css'
})
export class UpdateElementItemFormComponent implements OnInit {
  onSubmitDone = output<IItemDto>();
  onFormDataError = output<string[]>();

  $updateItem = input.required<IItemDto>();

  private itemService = inject(ItemService);

  $formData = signal<IV1ElementItemFormData | null>(null);
  $form = computed(() => {
    const item = this.$updateItem();
    const formData = this.$formData();
    if(!formData) return null;

    return new FormObject({
      name: new FormObjectControl({
        value: item.name,
        validators: [
          { fn: Validation.notNull, error: 'Required' },
          { fn: Validation.minLength(2), error: 'Must be minimum 2 characters long' },
        ]
      }),
      productionPrice: new FormObjectControl({
        value: item.productionPrice,
        validators: [
          { fn: Validation.notNull, error: 'Required' },
          { fn: Validation.min(0), error: 'Must be 0 or above' },
        ]
      }),
      productionTimeSeconds: new FormObjectControl({
        value: item.productionTimeSeconds,
        validators: [
          { fn: Validation.notNull, error: 'Required' },
          { fn: Validation.min(0), error: 'Must be 0 or above' },
        ]
      }),
      canExpire: new FormObjectControl({
        value: item.expirationTimeSeconds !== null,
        validators: []
      }),
      expirationTimeSeconds: new FormObjectControl({
        value: item.expirationTimeSeconds,
        validators: [{ fn: Validation.optional(Validation.min(0)), error: 'Must be 0 or above' }]
      }),
      unitPrice: new FormObjectControl({
        value: item.prices[item.prices.length - 1].unitValue,
        validators: [
          { fn: Validation.notNull, error: 'Required' },
          { fn: Validation.min(0), error: 'Must be 0 or above' },
        ]
      }),
      categoryId: new FormObjectControl({
        value: item.categoryId,
        validators: [{ fn: Validation.notNull, error: 'Required' }]
      }),
      unitOfMeasureId: new FormObjectControl({
        value: item.unitOfMeasureId,
        validators: [{ fn: Validation.notNull, error: 'Required' }]
      }),
    });
  });
  
  ngOnInit(): void {
    this.itemService.getElementItemFormData()
      .subscribe({
        next: x => {
          const errors: string[] = [];
          
          if(x.categories.length === 0)
            errors.push('At least 1 category must exist to create element item');
          
          if(x.unitsOfMeasure.length === 0)
            errors.push('At least 1 unit of measure must exist to create element item');
          
          errors.length === 0
            ? this.$formData.set(x)
            : this.onFormDataError.emit(errors);
        }
      });
  }

  onSubmit(ev: Event) {
    ev.preventDefault();
    const form = this.$form()!;
    const item = this.$updateItem()!;
    if(!form.valid) return;

    const dto: IV1UpdateElementItem = {
      itemId: item.id,
      name: form.controls.name.value!,
      productionPrice: form.controls.productionPrice.value!,
      productionTimeSeconds: form.controls.productionTimeSeconds.value!,
      expirationTimeSeconds: form.controls.expirationTimeSeconds.value,
      unitPrice: form.controls.unitPrice.value!,
      categoryId: form.controls.categoryId.value!,
      unitOfMeasureId: form.controls.unitOfMeasureId.value!
    }

    this.itemService.updateElementItem(dto)
      .subscribe({
        next: x => this.onSubmitDone.emit(x)
      })
  }
}
