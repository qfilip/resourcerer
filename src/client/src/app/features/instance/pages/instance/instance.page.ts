import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { map, Subscription, switchMap, tap } from 'rxjs';
import { InstanceService } from '../../services/instance.service';
import { ItemService } from '../../../item/services/item.service';
import { IItemDto } from '../../../../shared/dtos/interfaces';
import { ItemInstanceListComponent } from "../../components/item-instance-list/item-instance-list.component";

@Component({
  standalone: true,
  selector: 'app-instance.page',
  imports: [ItemInstanceListComponent],
  templateUrl: './instance.page.html',
  styleUrl: './instance.page.css'
})
export class InstancePage implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private itemService = inject(ItemService);
  private instanceService = inject(InstanceService);
  private sub!: Subscription;

  $item = signal<IItemDto | null>(null);
  
  ngOnInit(): void {
    this.sub = this.route.queryParams
    .pipe(
      map(params => params['itemId']),
      tap(itemId => {
        const item = this.itemService.$items().find(x => x.id === itemId)!;
        this.$item.set(item);
      }),
      switchMap(itemId => this.instanceService.getItemInstances(itemId))
    )
    .subscribe({ next: xs => console.log(xs)});
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
