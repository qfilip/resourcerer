import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { UserController } from '../../../controllers/user.controller';
import { PopupService } from '../../../services/popup.service';
import { UserService } from '../../../services/user.service';
import { IAppUserDto } from '../../../models/dtos/interfaces';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css'
})
export class LoginPage {
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
        next: jwt => {
          this.userService.setUser(jwt);
          this.router.navigate(['home']);
        }
      })
  }
}
