import { Component, signal, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TokenRefreshService } from './services/token-refresh.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit, OnDestroy {
  protected readonly title = signal('ElectricDashboard');

  constructor(private tokenRefreshService: TokenRefreshService) {}

  ngOnInit() {
    this.tokenRefreshService.startAutoRefresh();
  }

  ngOnDestroy() {
    this.tokenRefreshService.stopAutoRefresh();
  }
}
