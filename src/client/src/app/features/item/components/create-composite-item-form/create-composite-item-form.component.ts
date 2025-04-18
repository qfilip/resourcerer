import { Component, computed, effect, inject, OnInit, output, signal } from '@angular/core';
import { IItemDto, IV1CompositeItemFormData, IV1CreateCompositeItem, IV1ElementItemFormData } from '../../../../shared/dtos/interfaces';
import { ItemService } from '../../services/item.service';
import { FormObject, FormObjectControl } from '../../../../shared/utils/forms';
import { Validation } from '../../../../shared/utils/validation';
import { FormErrorComponent } from "../../../../shared/features/common-ui/components/form-error/form-error.component";

@Component({
  selector: 'app-create-composite-item-form',
  imports: [FormErrorComponent],
  templateUrl: './create-composite-item-form.component.html',
  styleUrl: './create-composite-item-form.component.css'
})
export class CreateCompositeItemFormComponent implements OnInit {
  onSubmitDone = output<IItemDto>();
  onFormDataError = output<string[]>();
  
  private itemService = inject(ItemService);
  
  itemsToUse: IItemDto[] = [];
  $itemsAvailable = signal<IItemDto[]>([]);
  $formData = signal<IV1CompositeItemFormData | null>(null);
  form: FormObject<CreateCompositeElementForm> | null = null;

  constructor() {
    effect(() => {
      const formData = this.$formData();
      if(!formData) return;
      
      this.form = new FormObject<CreateCompositeElementForm>({
        name: new FormObjectControl({
          value: '',
          validators: [
            { fn: Validation.notNull, error: 'Required' },
            { fn: Validation.minLength(2), error: 'Must be minimum 2 characters long' },
          ]
        }),
        productionPrice: new FormObjectControl({
          value: 0,
          validators: [
            { fn: Validation.notNull, error: 'Required' },
            { fn: Validation.min(0), error: 'Must be 0 or above' },
          ]
        }),
        productionTimeSeconds: new FormObjectControl({
          value: 0,
          validators: [
            { fn: Validation.notNull, error: 'Required' },
            { fn: Validation.min(0), error: 'Must be 0 or above' },
          ]
        }),
        canExpire: new FormObjectControl({
          value: false,
          validators: []
        }),
        expirationTimeSeconds: new FormObjectControl({
          value: null,
          validators: [{ fn: Validation.optional(Validation.min(0)), error: 'Must be 0 or above' }]
        }),
        unitPrice: new FormObjectControl({
          value: 0,
          validators: [
            { fn: Validation.notNull, error: 'Required' },
            { fn: Validation.min(0), error: 'Must be 0 or above' },
          ]
        }),
        itemIds: new FormObjectControl({
          value: [] as string[],
          validators: [{ fn: (xs: string[]) => xs.length > 0, error: 'At least 1 item must be specified' }]
        }),
        categoryId: new FormObjectControl({
          value: formData.categories[0].id,
          validators: [{ fn: Validation.notNull, error: 'Required' }]
        }),
        unitOfMeasureId: new FormObjectControl({
          value: formData.unitsOfMeasure[0].id,
          validators: [{ fn: Validation.notNull, error: 'Required' }]
        }),
      });
    });
  }
  
  ngOnInit() {
    this.itemService.getCompositeItemFormData()
      .subscribe({
        next: x => {
          const errors: string[] = [];
          
          if(x.items.length === 0)
            errors.push('At least 1 item must exist to create element item');

          if(x.categories.length === 0)
            errors.push('At least 1 category must exist to create element item');
          
          if(x.unitsOfMeasure.length === 0)
            errors.push('At least 1 unit of measure must exist to create element item');
          
          this.$itemsAvailable.set(x.items);

          errors.length === 0
            ? this.$formData.set(x)
            : this.onFormDataError.emit(errors);
        }
      });
  }

  addItemSlot() {
    const availiable = this.$itemsAvailable();
    if(availiable.length === 0) return;
    debugger
    console.log(availiable);
    this.itemsToUse.push(availiable.pop()!);
    console.log(availiable);
    this.$itemsAvailable.set(availiable);
    this.form!.controls.itemIds.data.value[this.itemsToUse.length - 1] = availiable[0].id;
  }

  setItem(useIndex: number, id: string) {
    const available = this.$itemsAvailable()
    const i = available.findIndex(x => x.id === id);
    this.form!.controls.itemIds.data.value[useIndex] = available[i].id;
    this.$itemsAvailable.update(xs => xs.splice(i, 1))
  }

  removeItemSlot(i: number) {
    this.$itemsAvailable.update(xs => xs.concat(this.itemsToUse.splice(i, 1)[0]));
  }

  onSubmit(e: Event) {
    e.preventDefault();
  }
}

type CreateCompositeElementForm = {
  name: string;
  productionPrice: number;
  productionTimeSeconds: number;
  canExpire: boolean;
  expirationTimeSeconds: number;
  unitPrice: number;
  itemIds: string[];
  categoryId: string;
  unitOfMeasureId: string;
}