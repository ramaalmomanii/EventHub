import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';



@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class Profile implements OnInit {
  user = signal<any>(null); 
  loading = signal(false);
  errorMsg = signal('');
  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.loadProfile();
  }
  loadProfile() {
    this.loading.set(true);
    this.authService.getMyProfile().subscribe({
      next: (res) => {
        this.user.set(res);
        this.loading.set(false);
      },
      error: (err) => {
        this.errorMsg.set(err?.error || 'حدث خطأ أثناء تحميل الملف الشخصي');
        this.loading.set(false);
      }

    });

  }
  editProfile() {
    // لاحقًا ممكن تفتح مودال أو صفحة تعديل
    alert('فتح صفحة تعديل الملف الشخصي');
  }


}
