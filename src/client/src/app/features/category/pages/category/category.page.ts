import { Component, inject, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { CategoryListComponent } from "../../components/category-list/category-list.component";

@Component({
  standalone: true,
  selector: 'app-category-page',
  imports: [CategoryListComponent],
  templateUrl: './category.page.html',
  styleUrl: './category.page.css'
})
export class CategoryPage implements OnInit {
  private categoryService = inject(CategoryService);

  ngOnInit(): void {
    this.categoryService.getAllCompanyCategories();
  }

}
