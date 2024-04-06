import { Component, OnInit, inject } from '@angular/core';
import { UserController } from '../../../../controllers/user.controller';
import { InMemoryCacheService } from '../../../../services/inmemory.cache.service';
import { UserService } from '../../../../services/user.service';
import { Observable } from 'rxjs';
import { IAppUserDto } from '../../../../models/dtos/interfaces';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'user-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent implements OnInit {
  private userService = inject(UserService);
  private userController = inject(UserController);
  private memoryCache = inject(InMemoryCacheService);
  
  companyUsers$: Observable<IAppUserDto[]> | null = null;

  ngOnInit() {
    const user = this.userService.user();
    if(!user) return;

    const usersData = this.userController.getAllCompanyUsers(user.company.id);
    this.companyUsers$ = this.memoryCache.companyUsers.retrieve(usersData);
  }
}
