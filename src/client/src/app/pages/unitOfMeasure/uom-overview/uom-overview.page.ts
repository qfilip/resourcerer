import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { IUnitOfMeasureDto, IV1CreateUnitOfMeasure, IV1EditUnitOfMeasure } from '../../../models/dtos/interfaces';
import { UnitOfMeasureController } from '../../../controllers/unitOfMeasure.controller';
import { Observable, switchMap } from 'rxjs';
import { CommonModule } from '@angular/common';
import { UomEditorComponent } from '../../../components/unitOfMeasure/uom-editor/uom-editor.component';
import { UomListComponent } from '../../../components/unitOfMeasure/uom-list/uom-list.component';

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

    const apiCall = () => this.uomController.create(dto);
    this.refreshList(apiCall);
  }

  edit(uom: IUnitOfMeasureDto) {
    const dto: IV1EditUnitOfMeasure = {
      id: uom.id,
      name: uom.name,
      symbol: uom.symbol
    };

    const apiCall = () => this.uomController.edit(dto);
    this.refreshList(apiCall);
  }

  delete(uom: IUnitOfMeasureDto) {
    const apiCall = () => this.uomController.delete(uom);
    this.refreshList(apiCall);
  }

  private refreshList(apiCall: () => Observable<IUnitOfMeasureDto>) {
    const companyId = this.appUser$()!.company.id;
    
    apiCall()
    .pipe(
      switchMap(_ => this.uomController.getCompanyUnitsOfMeasure(companyId)))
    .subscribe({
      next: xs => {
        this.setEditor(null);
        this.unitsOfMeasure$.set(xs);
      }
    });
  }
}
