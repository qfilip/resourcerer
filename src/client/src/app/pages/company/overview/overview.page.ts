import { Component, OnInit, computed, inject } from '@angular/core';
import { CompanyController } from '../../../controllers/company.controller';
import { IV1CompanyOverview } from '../../../models/dtos/interfaces';
import { Observable, tap } from 'rxjs';
import { UserService } from '../../../services/user.service';
import { CommonModule } from '@angular/common';
import { CategoryTreeComponent } from "../components/category-tree/category-tree.component";

@Component({
    selector: 'company-overview',
    standalone: true,
    templateUrl: './overview.page.html',
    styleUrl: './overview.page.css',
    imports: [CommonModule, CategoryTreeComponent]
})
export class CompanyOverviewPage implements OnInit {
  private companyController = inject(CompanyController);
  private userService = inject(UserService);
  user$ = computed(() => this.userService.user());
  
  overview$: Observable<IV1CompanyOverview> | null = null;
  
  ngOnInit() {
    const user = this.userService.user();
    if(!user) return;

    this.overview$ = this.companyController
      .getCompanyOverview(user.company.id)
      .pipe(tap(x => console.log(x)));
  }
}
