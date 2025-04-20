import { Component, output, signal, ViewChild } from '@angular/core';
import { DialogWrapperComponent } from '../../../../shared/features/common-ui/components/dialog-wrapper/dialog-wrapper.component';
import { IItemDto } from '../../../../shared/dtos/interfaces';
import { Observable, Subject } from 'rxjs';
import { FormErrorComponent } from "../../../../shared/features/common-ui/components/form-error/form-error.component";

@Component({
  selector: 'app-excerpt-dialog',
  imports: [DialogWrapperComponent, FormErrorComponent],
  templateUrl: './excerpt-dialog.component.html',
  styleUrl: './excerpt-dialog.component.css'
})
export class ExcerptDialogComponent {
  @ViewChild('wrapper') private wrapper!: DialogWrapperComponent;
  
  private result!: Subject<{ item: IItemDto, qty: number }[] | null>;
  private _$items = signal<IItemDto[]>([]);
  $items = this._$items.asReadonly();

  private _$recipe = signal<{ item: IItemDto, qty: number }[]>([]);
  $recipe = this._$recipe.asReadonly();
  errors = [] as string[];

  open(items: IItemDto[], recipe?: { item: IItemDto, qty: number }[]) {
    this._$items.set(items);
    if(recipe)
      this._$recipe.set(recipe);
    this.wrapper.open();
    
    this.result = new Subject();
    return this.result.asObservable();
  }

  addItem(item: IItemDto) {
    this._$items.update(xs => xs.filter(x => x.id !== item.id));
    this._$recipe.update(xs => xs.concat({ item: item, qty: 0 }))
  }

  updateRecipe(item: IItemDto, qty: number | string) {
    this._$recipe.update(xs =>
      xs.reduce((acc, x) => {
        if(x.item.id === item.id)
          x.qty = qty as number;

        return acc.concat(x)
      }, [] as {item: IItemDto, qty: number }[])
    )
  }

  remove(item: IItemDto) {
    this._$items.update(xs => xs.concat(item));
    this._$recipe.update(xs => xs.filter(x => x.item.id !== item.id));
  }

  create() {
    const xs = this._$recipe();
    if(!xs.every(x => x.qty > 0)) {
      this.errors = ['All items must have value above 0'];
      return;
    }

    this.result.next(this._$recipe());
    this.result.complete();
    this.wrapper.close();
  }

  close() {
    this.result.next(null);
    this.result.complete();
    this.wrapper.close();
  }
}
