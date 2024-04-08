import { Component, OnInit, inject, signal } from '@angular/core';
import { InMemoryCacheService } from '../../../../services/inmemory.cache.service';
import { UserService } from '../../../../services/user.service';
import { Observable, iif } from 'rxjs';
import { IAppUserDto } from '../../../../models/dtos/interfaces';
import { CommonModule } from '@angular/common';
import { UserEditComponent } from "../user-edit/user-edit.component";
import { DialogWrapperComponent } from "../../../../components/dialog-wrapper/dialog-wrapper.component";
import { RegisterUserComponent } from "../register-user/register-user.dialog";

@Component({
    selector: 'user-list',
    standalone: true,
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.css',
    imports: [CommonModule, UserEditComponent, DialogWrapperComponent, RegisterUserComponent]
})
export class UserListComponent implements OnInit {
  private userService = inject(UserService);
  private memoryCache = inject(InMemoryCacheService);
  
  companyUsers$: Observable<IAppUserDto[]> | null = null;
  currentUser: IAppUserDto | null = null;
  selectedUser$ = signal<IAppUserDto | null>(null);

  ngOnInit() {
    const user = this.userService.user();
    if(!user) return;
    
    this.currentUser = user;
    this.companyUsers$ = this.memoryCache.companyUsers.retrieve(user.company.id);
  }
}
