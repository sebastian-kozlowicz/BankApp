import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-nav-registration-menu',
  templateUrl: './nav-registration-menu.component.html',
  styleUrls: ['./nav-registration-menu.component.css']
})
export class NavRegistrationMenuComponent implements OnInit {

  constructor(public authService: AuthService) { }

  ngOnInit(): void {
  }

}
