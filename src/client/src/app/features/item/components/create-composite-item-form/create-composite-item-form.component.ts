import { Component, computed, effect, inject, OnInit, output, signal } from '@angular/core';
import { IItemDto, IV1CompositeItemFormData, IV1CreateCompositeItem, IV1ElementItemFormData } from '../../../../shared/dtos/interfaces';
import { ItemService } from '../../services/item.service';
import { FormObject, FormObjectControl } from '../../../../shared/utils/forms';
import { Validation } from '../../../../shared/utils/validation';
import { FormErrorComponent } from "../../../../shared/features/common-ui/components/form-error/form-error.component";
import { Functions } from '../../../../shared/utils/functions';

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
  
  recipe: { item: IItemDto, qty: number}[] = [];
  itemsLeft: IItemDto[] = [];
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

          this.itemsLeft = x.items;
          errors.length === 0
            ? this.$formData.set(x)
            : this.onFormDataError.emit(errors);
        }
      });
  }

  addItem(item: IItemDto) {
    this.recipe.push({ item: item, qty: 0 });
    this.itemsLeft = this.itemsLeft.filter(x => x.id !== item.id);
  }

  removeItem(item: IItemDto) {
    this.itemsLeft.push(item);
    this.recipe = this.recipe.filter(x => x.item.id !== item.id);
  }

  setQuantity(x: { item: IItemDto, qty: number}, qty: number | string) {
    x.qty = qty as number;
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