import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
  standalone: false,
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  constructor(public readonly authService: AuthService) {
  }

}
