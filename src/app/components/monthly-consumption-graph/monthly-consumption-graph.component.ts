import { Component, Input, ViewChild, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-monthly-consumption-graph',
  template: `
    <div class="chart-wrapper">
      <canvas #chartCanvas></canvas>
    </div>
  `,
  styleUrls: ['./monthly-consumption-graph.component.css']
})
export class MonthlyConsumptionGraphComponent implements AfterViewInit, OnDestroy {
  @Input() chartData: any[] = [];
  @Input() loading = false;
  
  @ViewChild('chartCanvas') chartCanvas!: ElementRef<HTMLCanvasElement>;
  
  private chart: Chart | null = null;
  
  ngAfterViewInit() {
    this.createChart();
  }
  
  ngOnDestroy() {
    if (this.chart) {
      this.chart.destroy();
    }
  }
  
  private createChart() {
    const canvas = this.chartCanvas?.nativeElement;
    if (!canvas) {
      console.log('Canvas element not found');
      return;
    }
    
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      console.error('Canvas context not available');
      return;
    }
    
    // Create chart with monthly data
    this.chart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        datasets: []
      },
      options: {
        responsive: true,
        maintainAspectRatio: true,
        scales: {
          x: {
            title: {
              display: true,
              text: 'Month'
            }
          },
          y: {
            type: 'linear' as const,
            display: true,
            position: 'left' as const,
            title: {
              display: true,
              text: 'kWh'
            },
            beginAtZero: true
          }
        },
        plugins: {
          legend: {
            position: 'top' as const
          },
          tooltip: {
            callbacks: {
              label: (context) => {
                const label = context.dataset.label || '';
                const value = context.raw as number;
                return `${label}: ${value.toLocaleString('en-US')}`;
              }
            }
          }
        }
      }
    });
    
    this.updateChart();
  }
  
  updateChart() {
    if (!this.chart) {
      console.log('Chart not initialized');
      return;
    }
    
    if (this.chartData.length === 0) {
      console.log('No data to display');
      this.clearChart();
      return;
    }
    
    // Prepare the datasets for the last 3 years
    const yearData: { [year: string]: { consumption: number[] } } = {};
    
    // Process the data to group by year
    this.chartData.forEach(entry => {
      if (!yearData[entry.year.toString()]) {
        yearData[entry.year.toString()] = { consumption: Array(12).fill(0) };
      }
      
      // Convert month to index (0-11) and set consumption value
      if (entry.month && entry.consumption !== undefined) {
        const monthIndex = entry.month - 1; // Month from 1-12 to 0-11
        yearData[entry.year.toString()].consumption[monthIndex] = entry.consumption;
      }
    });
    
    // Create datasets for each year
    const datasets = Object.keys(yearData)
      .sort((a, b) => parseInt(a) - parseInt(b)) // Sort years ascending
      .map((year, index) => {
        const yearColor = this.getColorForYear(year, index);
        return {
          label: `Year ${year}`,
          data: yearData[year].consumption,
          backgroundColor: yearColor,
          borderColor: yearColor,
          borderWidth: 1
        };
      });
    
    // Update chart data
    this.chart.data.datasets = datasets;
    this.chart.update();
  }
  
  private getColorForYear(year: string, index: number) {
    // Generate distinct colors for each year
    const hue = (index * 120) % 360; // Spacing colors evenly around the hue wheel
    return `hsla(${hue}, 70%, 60%, 0.7)`;
  }
  
  private clearChart() {
    if (this.chart) {
      this.chart.data.labels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
      this.chart.data.datasets = [];
      this.chart.update();
    }
  }
}