import { Component, effect, inject, signal } from '@angular/core';

import { LoaderService } from '../../services/loader.service';

@Component({
  standalone: true,
  selector: 'app-loader',
  imports: [],
  templateUrl: './loader.component.html',
  styleUrl: './loader.component.css'
})
export class LoaderComponent {
  private service = inject(LoaderService);
  private _$visible = signal<boolean>(false);
  private _$message = signal<string>('loading...');

  $visible = this._$visible.asReadonly();
  $message = this._$message.asReadonly();

  constructor() {
    effect(() => this._$visible.set(this.service.$isLoading()));
    effect(() => this._$message.set(this.service.$message()));
  }
}
