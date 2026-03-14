import { Component, input, } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Forecast } from '../../models/forecast.model';

@Component({
  selector: 'app-forecast-display',
  imports: [CommonModule],
  templateUrl: './forecast-display.component.html',
  styleUrls: ['./forecast-display.component.css'],
})
export class ForecastDisplay {
  forecast = input<Forecast | null>();
}
