import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { LoginDto } from '../../models/user';
import { Footer } from '../layout/footer/footer';
@Component({
  standalone: true,
  selector: 'app-login',
  imports: [RouterLink, FormsModule, ReactiveFormsModule, Footer],
  templateUrl: './login.html',
  styleUrl: './login.scss'

})

export class Login {
  loginData: LoginDto = { email: '', password: '' };
  constructor(private authService: AuthService, private router: Router) { }
  isLoading = false;
  onSubmit() {
    this.isLoading = true;
    this.authService.login(this.loginData).subscribe({
      next: (res) => {
        this.isLoading = false;
        //console.log('✅ Logged in:', res);
        localStorage.setItem('token', res.accessToken);
        //alert('تم تسجيل الدخول بنجاح ✅');
        this.router.navigate(['/dashboard']);
        // TODO: Navigate to home/dashboard
      },
      error: (err) => {
        this.isLoading = false;
        if (err.status === 401) {
          alert('الحساب غير موجود أو كلمة المرور خاطئة.\nإنشئ حساب جديد إذا لم يكن لديك واحد.');
          // ممكن تعملي Navigation إلى صفحة التسجيل
           this.router.navigate(['/register']);
        } else {
          alert('حدث خطأ أثناء تسجيل الدخول.');
        }
      }
    });
  }
}
