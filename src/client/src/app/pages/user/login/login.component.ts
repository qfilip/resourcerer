import { Component, inject } from '@angular/core';
import { UserController } from '../../../controllers/user.controller';
import { PopupService } from '../../../services/popup.service';
import { IAppUserDto } from '../../../models/dtos/interfaces';
import { UserService } from '../../../services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  router = inject(Router);
  popup = inject(PopupService);
  userService = inject(UserService);
  userController = inject(UserController);
  
  submit(ev: Event, name: string, password: string) {
    ev.preventDefault();
    const notEmpty = (x: string) => x && x.length > 0;
    let errors = [];

    if(!notEmpty(name)) {
      errors.push('Username cannot be empty');
    }

    if(!notEmpty(password)) {
      errors.push('Password cannot be empty');
    }

    if(errors.length > 0) {
      errors.forEach(x => this.popup.warning(x));
      
      return;
    }

    const dto = { name: name, password: password } as IAppUserDto;
    this.userController.login(dto)
      .subscribe({
        next: x => {
          this.userService.setUser(x);
          this.router.navigate(['home']);
        },
        error: e => console.log(e)
      })
  }
}
