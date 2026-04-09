import { Component, signal, OnInit, OnDestroy, inject, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TokenRefreshService } from './services/token-refresh.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class App implements OnInit, OnDestroy {
  private tokenRefreshService = inject(TokenRefreshService);

  ngOnInit() {
    this.tokenRefreshService.startAutoRefresh();
  }

  ngOnDestroy() {
    this.tokenRefreshService.stopAutoRefresh();
  }
}
