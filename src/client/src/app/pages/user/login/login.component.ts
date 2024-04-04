import { Component, inject } from '@angular/core';
import { UserController } from '../../../controllers/user.controller';
import { PopupService } from '../../../services/popup.service';
import { IPopup } from '../../../models/components/IPopup';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  controller = inject(UserController);
  popup = inject(PopupService);
  
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
      const popups = errors.map(x => ({ message: x, type: 'warning' } as IPopup))
      this.popup.many(popups);
      
      return;
    }
  }
}
