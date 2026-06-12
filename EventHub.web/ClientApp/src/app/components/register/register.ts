import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service'; 
import { Router } from '@angular/router';
import { Footer } from '../layout/footer/footer';

@Component({
  standalone: true,
  selector: 'app-register',
  templateUrl: './register.html',
  styleUrl: './register.scss',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, Footer] 
})
export class Register {
  registerForm: FormGroup;
  loading: boolean = false;
  errorMsg: string = '';
  successMsg: string = '';

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.registerForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });

  
   
  }
  goToLogin() {
    this.router.navigate(['/login']);
  }
  onSubmit() {
    if (this.registerForm.invalid) return;

    this.loading = true;
    this.errorMsg = '';
    this.successMsg = '';

    this.authService.register(this.registerForm.value).subscribe({
      next: (res: any) => {
        this.successMsg = 'تم تسجيلك بنجاح 🎉';
        this.registerForm.reset();
        this.loading = false;
        setTimeout(() => this.router.navigate(['/login']), 1500); // تحويل تلقائي
      },
      error: (err: any) => {
        // عرض رسالة الخطأ الصحيحة بدل [object Object]
        this.errorMsg = err?.error?.message || 'حدث خطأ أثناء التسجيل ❌';
        this.loading = false;
      }
    });
  }


}
