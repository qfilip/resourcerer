import { Component, computed, inject, signal } from '@angular/core';
import { UomListComponent } from "../components/uom-list/uom-list.component";
import { UserService } from '../../../services/user.service';
import { UomEditorComponent } from "../components/uom-editor/uom-editor.component";
import { IUnitOfMeasureDto } from '../../../models/dtos/interfaces';

@Component({
    selector: 'uom-overview',
    standalone: true,
    templateUrl: './uom-overview.page.html',
    styleUrl: './uom-overview.page.css',
    imports: [UomListComponent, UomEditorComponent]
})
export class UomOverviewPage {
  private userService = inject(UserService);
  appUser$ = computed(() => this.userService.user());

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

  setEditor(mode: null | 'Create' | 'Edit', uom: IUnitOfMeasureDto | null = null) {
    this.editorMode$.set(mode);
    this.selectedUom$.set(uom);
  }
}
