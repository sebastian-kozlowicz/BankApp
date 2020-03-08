import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-nav-login-menu',
  templateUrl: './nav-login-menu.component.html',
  styleUrls: ['./nav-login-menu.component.css']
})
export class NavLoginMenuComponent implements OnInit {

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }
}
