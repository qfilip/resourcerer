import { inject, Injectable, signal } from "@angular/core";
import { IInstanceDto, IV1InstanceInfo } from "../../../shared/dtos/interfaces";
import { InstanceApiService } from "./instance.api.service";
import { tap } from "rxjs";

@Injectable({ providedIn: 'root' })
export class InstanceService {
  private apiService = inject(InstanceApiService);
  
  private _$instancesInfo = signal<IV1InstanceInfo[]>([]);
  private _$selectedInstance = signal<IInstanceDto | null>(null);
  
  $instancesInfo = this._$instancesInfo.asReadonly();
  $selectedInstance = this._$selectedInstance.asReadonly();
  
  setSelected = (x: IInstanceDto) => this._$selectedInstance.set(x);

  getItemInstances(itemId: string) {
    return this.apiService.getItemInstancesInfo(itemId)
      .pipe(tap(xs => this._$instancesInfo.set(xs)));
  }
}