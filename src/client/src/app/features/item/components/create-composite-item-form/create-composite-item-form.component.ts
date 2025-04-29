import { Component, computed, effect, inject, OnInit, output, signal, ViewChild } from '@angular/core';
import { IItemDto, IV1CompositeItemFormData, IV1CreateCompositeItem, IV1ElementItemFormData } from '../../../../shared/dtos/interfaces';
import { ItemService } from '../../services/item.service';
import { FormObject, FormObjectControl } from '../../../../shared/utils/forms';
import { Validation } from '../../../../shared/utils/validation';
import { FormErrorComponent } from "../../../../shared/features/common-ui/components/form-error/form-error.component";
import { Functions } from '../../../../shared/utils/functions';
import { ExcerptDialogComponent } from "../excerpt-dialog/excerpt-dialog.component";

@Component({
  selector: 'app-create-composite-item-form',
  imports: [FormErrorComponent, ExcerptDialogComponent],
  templateUrl: './create-composite-item-form.component.html',
  styleUrl: './create-composite-item-form.component.css'
})
export class CreateCompositeItemFormComponent implements OnInit {
  @ViewChild('excerptDialog') private excerptDialog!: ExcerptDialogComponent;
  onSubmitDone = output<IItemDto>();
  onFormDataError = output<string[]>();
  
  private itemService = inject(ItemService);
  
  items: IItemDto[] = [];
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
        recipe: new FormObjectControl({
          value: [] as {item: IItemDto, qty: number }[],
          validators: [{ fn: this.recipeValidator, error: 'Composite must have at least 1 element' }]
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

          this.items = x.items;
          errors.length === 0
            ? this.$formData.set(x)
            : this.onFormDataError.emit(errors);
        }
      });
  }

  openRecipeDialog() {
    this.excerptDialog.open(this.items, this.form!.controls.recipe.value)
    .subscribe({
      next: v => {
        if(v) {
          this.form!.controls.recipe.setValue(v);
        }
      }
    });
  }

  onSubmit(e: Event) {
    e.preventDefault();
    if(!this.form?.valid) {
      return;
    }

    const cs = this.form.controls;
    const dto: IV1CreateCompositeItem = {
      name: cs.name.value!,
      categoryId: cs.categoryId.value!,
      unitOfMeasureId: cs.unitOfMeasureId.value,
      preparationTimeSeconds: cs.productionTimeSeconds.value,
      expirationTimeSeconds: cs.expirationTimeSeconds.value,
      unitPrice: cs.unitPrice.value,
      excerptMap: this.mapRecipeDictionary(cs.recipe.value)
    }

    this.itemService.createCompositeItem(dto)
      .subscribe({
        next: v => this.onSubmitDone.emit(v)
      });
  }

  private recipeValidator = (xs: {item: IItemDto, qty: number}[]) => xs.length > 0;
  private mapRecipeDictionary(xs: {item: IItemDto, qty: number}[]) {
    const dict = {} as { [key: string]: number };
    xs.forEach(x => dict[x.item.id] = x.qty)

    return dict;
  }
}

type CreateCompositeElementForm = {
  name: string;
  productionPrice: number;
  productionTimeSeconds: number;
  canExpire: boolean;
  expirationTimeSeconds: number;
  unitPrice: number;
  recipe: {item: IItemDto, qty: number}[];
  categoryId: string;
  unitOfMeasureId: string;
}