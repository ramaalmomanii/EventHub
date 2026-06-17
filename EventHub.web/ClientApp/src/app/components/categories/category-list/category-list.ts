import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../../../services/category';
import { Category, CategoryCreateDto, CategoryUpdateDto } from '../../../models/category';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './category-list.html',
  styleUrl: './category-list.scss'
})
export class CategoryList implements OnInit {
  private categoryService = inject(CategoryService);

  categories: Category[] = [];
  isLoading = false;
  showModal = false;
  isEditMode = false;
  selectedCategory: Category | null = null;

  form: CategoryCreateDto = { name: '', description: '' };

  ngOnInit() { this.load(); }

  load() {
    this.isLoading = true;
    this.categoryService.getAll().subscribe({
      next: data => { this.categories = data; this.isLoading = false; },
      error: () => this.isLoading = false
    });
  }

  openAdd() {
    this.isEditMode = false;
    this.form = { name: '', description: '' };
    this.showModal = true;
  }

  openEdit(cat: Category) {
    this.isEditMode = true;
    this.selectedCategory = cat;
    this.form = { name: cat.name, description: cat.description };
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.selectedCategory = null;
  }

  save() {
    if (this.isEditMode && this.selectedCategory) {
      this.categoryService.update(this.selectedCategory.id, this.form).subscribe({
        next: () => { this.load(); this.closeModal(); },
        error: err => alert(err.error?.error ?? 'Error')
      });
    } else {
      this.categoryService.create(this.form).subscribe({
        next: () => { this.load(); this.closeModal(); },
        error: err => alert(err.error?.error ?? 'Error')
      });
    }
  }

  delete(cat: Category) {
    if (!confirm(`Delete "${cat.name}"?`)) return;
    this.categoryService.delete(cat.id).subscribe({
      next: () => this.load()
    });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }
}
