import { Component, ViewChild, inject, signal } from '@angular/core';
import { UserListComponent } from "../components/user-list/user-list.component";
import { Observable } from 'rxjs';
import { IAppUserDto } from '../../../models/dtos/interfaces';
import { InMemoryCacheService } from '../../../services/inmemory.cache.service';
import { UserService } from '../../../services/user.service';
import { RegisterUserComponent } from '../components/register-user/register-user.component';
import { UserEditComponent } from '../components/user-edit/user-edit.component';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'user-management',
    standalone: true,
    templateUrl: './user-management.page.html',
    styleUrl: './user-management.page.css',
    imports: [CommonModule, UserListComponent, RegisterUserComponent, UserEditComponent]
})
export class UserManagementPage {
  @ViewChild('userList') userList!: UserListComponent;
  private userService = inject(UserService);
  
  component$ = signal<null | 'edit' | 'register'>(null);
  currentUser: IAppUserDto | null = null;
  selectedUser$ = signal<IAppUserDto | null>(null);

  ngOnInit() {
    const user = this.userService.user();
    if(!user) return;
    
    this.currentUser = user;
  }

  setRegisterComponent() {
    this.component$.set('register');
  }

  setEditComponent(x: IAppUserDto) {
    this.selectedUser$.set(x);
    this.component$.set('edit');
  }

  onUserEdited() {
    this.userList.refreshList();
  }

  onUserRegistered() {
    this.userList.refreshList();
  }
}
