import { Component, OnInit, inject } from '@angular/core';
import { UserController } from '../../../../controllers/user.controller';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent implements OnInit {
  private userController = inject(UserController);

  ngOnInit() {
    
  }
}
