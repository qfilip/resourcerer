import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CompanyController } from '../../../controllers/company.controller';
import { IV1CompanyOverview } from '../../../models/dtos/interfaces';
import { Observable } from 'rxjs';
import { UserService } from '../../../services/user.service';
import { CommonModule } from '@angular/common';
import { CategoryTreeComponent } from '../../../components/company/category-tree/category-tree.component';

@Component({
    selector: 'company-overview',
    standalone: true,
    templateUrl: './company.overview.page.html',
    styleUrl: './company.overview.page.css',
    imports: [CommonModule, CategoryTreeComponent]
})
export class CompanyOverviewPage implements OnInit {
  private companyController = inject(CompanyController);
  private userService = inject(UserService);
  user$ = computed(() => this.userService.user());
  overview$: Observable<IV1CompanyOverview> | null = null;
  dataView$ = signal<'categories' | 'employees'>('categories');
  
  ngOnInit() {
    const user = this.userService.user();
    if(!user) return;

    this.overview$ = this.companyController
      .getCompanyOverview(user.company.id);
  }

  onDataViewSelected(ev: any) {
    const value = ev.target.value as 'categories' | 'employees';
    this.dataView$.set(value);
  }
}
