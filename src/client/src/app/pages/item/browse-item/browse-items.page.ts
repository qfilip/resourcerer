import { Component, inject, OnInit } from '@angular/core';
import { ItemController } from '../../../controllers/item.controller';
import { UserService } from '../../../services/user.service';
import { BehaviorSubject, combineLatest, filter, map, Observable, tap } from 'rxjs';
import { IV1ItemShoppingDetails } from '../../../models/dtos/interfaces';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'browse-items',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './browse-items.page.html',
  styleUrl: './browse-items.page.css'
})
export class BrowseItemsPage implements OnInit {

  private userService = inject(UserService)
  private itemController = inject(ItemController)
  itemShoppingDetails$!: Observable<IV1ItemShoppingDetails[]>;
  page$ = new BehaviorSubject<number>(1);
  pageSize$ = new BehaviorSubject<number>(20);
  search$ = new BehaviorSubject<string>('');
  pageCount = 0;
  
  ngOnInit(): void {
    const user = this.userService.user();
    this.itemShoppingDetails$ = combineLatest({
      items: this.itemController.getItemsShoppingDetails(user!.company.id),
      page: this.page$,
      pageSize: this.pageSize$,
      search: this.search$
    })
    .pipe(
      tap(d => {
        this.pageCount = Math.ceil(d.items.length / d.pageSize);
        if(d.page < 1) {
          this.page$.next(1);
        }
        else if(d.page > this.pageCount) {
          this.page$.next(this.pageCount);
        }
      }),
      filter(d => d.page > 0 && d.page <= this.pageCount),
      map(d => {
        const filteredItems = d.items
          .filter(x => x.categoryName.includes(d.search));
          
          return filteredItems.slice(d.page - 1, d.page + d.pageSize - 1);
      })
    );
  }

}
