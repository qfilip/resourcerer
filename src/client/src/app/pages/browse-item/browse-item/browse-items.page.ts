import { Component, inject, OnInit } from '@angular/core';
import { ItemController } from '../../../controllers/item.controller';
import { UserService } from '../../../services/user.service';
import { Observable } from 'rxjs';
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
  ngOnInit(): void {
    const user = this.userService.user();
    this.itemShoppingDetails$ =  this.itemController
      .getItemsShoppingDetails(user!.company.id);
  }

}
