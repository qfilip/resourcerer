import { Component, inject, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-category',
  imports: [],
  templateUrl: './category.page.html',
  styleUrl: './category.page.css'
})
export class CategoryPage implements OnInit {
  private categoryService = inject(CategoryService);

  ngOnInit(): void {
    this.categoryService.getAllCompanyCategories();
  }

}
