import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { IAppUserDto } from '../../../../shared/dtos/interfaces';
import { PopupService } from '../../../../shared/features/common-ui/services/popup.service';
import { UserService } from '../../services/user.service';

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
      errors.forEach(x => this.popup.warn(x));
      
      return;
    }

    const dto = { name: name, password: password } as IAppUserDto;
    this.userService.login(dto)
      .subscribe({
        next: _ => {
          this.router.navigate(['home']);
        }
      })
  }
}
