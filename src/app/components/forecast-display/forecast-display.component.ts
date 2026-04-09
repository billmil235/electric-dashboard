import { Component, input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Forecast } from '../../models/forecast.model';

@Component({
  selector: 'app-forecast-display',
  imports: [CommonModule],
  templateUrl: './forecast-display.component.html',
  styleUrls: ['./forecast-display.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ForecastDisplay {
  forecast = input<Forecast | null>();
}
