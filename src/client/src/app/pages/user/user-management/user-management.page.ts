import { Component, ViewChild, inject, signal } from '@angular/core';
import { IAppUserDto } from '../../../models/dtos/interfaces';
import { UserService } from '../../../services/user.service';
import { CommonModule } from '@angular/common';
import { UserListComponent } from '../../../components/user/user-list/user-list.component';
import { RegisterUserComponent } from '../../../components/user/register-user/register-user.component';
import { UserEditComponent } from '../../../components/user/user-edit/user-edit.component';

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
