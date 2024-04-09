import { Component, inject, signal } from '@angular/core';
import { UserListComponent } from "../components/user-list/user-list.component";
import { Observable } from 'rxjs';
import { IAppUserDto } from '../../../models/dtos/interfaces';
import { InMemoryCacheService } from '../../../services/inmemory.cache.service';
import { UserService } from '../../../services/user.service';
import { RegisterUserComponent } from '../components/register-user/register-user.component';
import { UserEditComponent } from '../components/user-edit/user-edit.component';

@Component({
    selector: 'user-management',
    standalone: true,
    templateUrl: './user-management.page.html',
    styleUrl: './user-management.page.css',
    imports: [UserListComponent, RegisterUserComponent, UserEditComponent]
})
export class UserManagementPage {
  private userService = inject(UserService);
  private memoryCache = inject(InMemoryCacheService);
  
  component$ = signal<null | 'edit' | 'register'>(null);
  companyUsers$: Observable<IAppUserDto[]> | null = null;
  currentUser: IAppUserDto | null = null;
  selectedUser$ = signal<IAppUserDto | null>(null);

  ngOnInit() {
    const user = this.userService.user();
    if(!user) return;
    
    this.currentUser = user;
    this.companyUsers$ = this.memoryCache.companyUsers.retrieve(user.company.id);
  }

  setRegisterComponent() {
    this.component$.set('register');
  }

  setEditComponent(x: IAppUserDto) {
    this.selectedUser$.set(x);
    this.component$.set('edit');
  }
}
