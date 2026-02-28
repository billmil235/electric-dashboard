import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Forecast } from '../../models/forecast.model';

@Component({
  selector: 'app-forecast-display',
  imports: [CommonModule],
  templateUrl: './forecast-display.html',
  styleUrls: ['./forecast-display.css'],
})
export class ForecastDisplay {
  @Input() forecast: Forecast | null = null;
}
