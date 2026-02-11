import { Component } from '@angular/core';
import { LoggedInLayout } from "../logged-in-layout/logged-in-layout";

@Component({
  selector: 'app-reports',
  templateUrl: './reports.html',
  styleUrl: './reports.css',
  standalone: true,
  imports: [LoggedInLayout]
})
export class Reports {
  constructor() { }
}
