import { Component } from '@angular/core';
import { AuthService, LoginDto } from '../../services/auth.service';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [RouterLink, FormsModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'

})

export class Login {
  loginData: LoginDto = { email: '', password: '' };
  constructor(private authService: AuthService, private router: Router) { }

  onSubmit() {
    this.authService.login(this.loginData).subscribe({
      next: (res) => {
        console.log('✅ Logged in:', res);
        localStorage.setItem('token', res.token);
        alert('تم تسجيل الدخول بنجاح ✅');
        this.router.navigate(['me']);
        // TODO: Navigate to home/dashboard
      },
      error: (err) => {
        if (err.status === 401) {
          alert('الحساب غير موجود أو كلمة المرور خاطئة.\nإنشئ حساب جديد إذا لم يكن لديك واحد.');
          // ممكن تعملي Navigation إلى صفحة التسجيل
          // this.router.navigate(['/register']);
        } else {
          alert('حدث خطأ أثناء تسجيل الدخول.');
        }
      }
    });
  }
}
