import { Component, output, signal, ViewChild } from '@angular/core';
import { DialogWrapperComponent } from '../../../../shared/features/common-ui/components/dialog-wrapper/dialog-wrapper.component';
import { IItemDto } from '../../../../shared/dtos/interfaces';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-excerpt-dialog',
  imports: [DialogWrapperComponent],
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

  open(items: IItemDto[]) {
    this._$items.set(items);
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
