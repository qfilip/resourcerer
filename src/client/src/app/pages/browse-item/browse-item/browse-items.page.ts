import { Component, inject, OnInit } from '@angular/core';
import { ItemController } from '../../../controllers/item.controller';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'browse-items',
  standalone: true,
  imports: [],
  templateUrl: './browse-items.page.html',
  styleUrl: './browse-items.page.css'
})
export class BrowseItemsPage implements OnInit {

  private userService = inject(UserService)
  private itemController = inject(ItemController)
  ngOnInit(): void {
    const user = this.userService.user();
    this.itemController.getItemsShoppingDetails(user!.company.id)
      .subscribe(xs => console.log(xs));
  }

}
