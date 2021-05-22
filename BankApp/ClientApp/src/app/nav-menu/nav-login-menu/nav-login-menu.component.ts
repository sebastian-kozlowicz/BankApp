import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-nav-login-menu',
  templateUrl: './nav-login-menu.component.html',
  styleUrls: ['./nav-login-menu.component.css']
})
export class NavLoginMenuComponent implements OnInit {

  constructor(public authService: AuthService) { }

  ngOnInit(): void {
  }
}
