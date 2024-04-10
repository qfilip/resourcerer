import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { UomListComponent } from "../components/uom-list/uom-list.component";
import { UserService } from '../../../services/user.service';
import { UomEditorComponent } from "../components/uom-editor/uom-editor.component";
import { IUnitOfMeasureDto, IV1CreateUnitOfMeasure } from '../../../models/dtos/interfaces';
import { UnitOfMeasureController } from '../../../controllers/unitOfMeasure.controller';
import { Observable, switchMap } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'uom-overview',
    standalone: true,
    templateUrl: './uom-overview.page.html',
    styleUrl: './uom-overview.page.css',
    imports: [CommonModule, UomListComponent, UomEditorComponent]
})
export class UomOverviewPage implements OnInit {
  private userService = inject(UserService);
  private uomController = inject(UnitOfMeasureController);
  
  appUser$ = computed(() => this.userService.user());
  unitsOfMeasure$ = signal<IUnitOfMeasureDto[] | null>(null);

  selectedUom$ = signal<IUnitOfMeasureDto | null>(null);
  editorMode$ = signal<null | 'Create' | 'Edit'>(null);

  editor$ = computed(() => {
    const uom = this.selectedUom$();
    const mode = this.editorMode$();
    const defaultValue = { name: '', symbol: '' } as IUnitOfMeasureDto;

    if(mode === 'Create' && uom === null) {
      return { visible: true, mode: mode, data: defaultValue }
    }
    else if(mode === 'Edit' && uom !== null) {
      return { visible: true, mode: mode, data: uom }
    }
    else {
      return { visible: false, mode: mode, data: defaultValue }
    }
  });

  ngOnInit() {
    const user = this.userService.user();
    if(!user) return;

    this.uomController
      .getCompanyUnitsOfMeasure(user.company.id)
      .subscribe({
        next: xs => this.unitsOfMeasure$.set(xs)
      });
  }

  setEditor(mode: null | 'Create' | 'Edit', uom: IUnitOfMeasureDto | null = null) {
    this.editorMode$.set(mode);
    this.selectedUom$.set(uom);
  }

  create(uom: IUnitOfMeasureDto) {
    const companyId = this.appUser$()!.company.id;
    
    const dto: IV1CreateUnitOfMeasure = {
      companyId: companyId,
      name: uom.name,
      symbol: uom.symbol
    };

    this.uomController.createUnitOfMeasure(dto)
      .pipe(
        switchMap(_ => this.uomController.getCompanyUnitsOfMeasure(companyId)))
      .subscribe({
        next: xs => {
          this.setEditor(null);
          this.unitsOfMeasure$.set(xs);
        }
      })
  }

  edit(uom: IUnitOfMeasureDto) {
    
  }
}
