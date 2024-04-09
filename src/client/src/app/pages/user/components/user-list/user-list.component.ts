import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { InMemoryCacheService } from '../../../../services/inmemory.cache.service';
import { UserService } from '../../../../services/user.service';
import { Observable } from 'rxjs';
import { IAppUserDto } from '../../../../models/dtos/interfaces';
import { CommonModule } from '@angular/common';
import { UserEditComponent } from "../user-edit/user-edit.component";
import { DialogWrapperComponent } from "../../../../components/dialog-wrapper/dialog-wrapper.component";
import { RegisterUserComponent } from "../register-user/register-user.component";

@Component({
    selector: 'user-list',
    standalone: true,
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.css',
    imports: [CommonModule, UserEditComponent, DialogWrapperComponent, RegisterUserComponent]
})
export class UserListComponent implements OnInit {
  @Input({ required: true }) currentUser!: IAppUserDto;
  @Output() onUserEdit = new EventEmitter<IAppUserDto>();
  
  private userService = inject(UserService);
  private memoryCache = inject(InMemoryCacheService);
  
  companyUsers$: Observable<IAppUserDto[]> | null = null;

  ngOnInit() {
    this.companyUsers$ = this.memoryCache.companyUsers.retrieve(this.currentUser.company.id);
  }
}
