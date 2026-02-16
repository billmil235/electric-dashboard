import { Component, Input, ViewChild, ElementRef, AfterViewInit, OnDestroy, OnChanges, SimpleChanges } from '@angular/core';
import Chart from 'chart.js/auto';
import { ElectricBill } from '../../models/electric-bill.model';

@Component({
  selector: 'app-monthly-consumption-graph',
  template: `
    <div class="chart-wrapper">
      <canvas #chartCanvas></canvas>
    </div>
  `,
  styleUrls: ['./monthly-consumption-graph.component.css']
})
export class MonthlyConsumptionGraphComponent implements AfterViewInit, OnDestroy, OnChanges {
  @Input() bills: ElectricBill[] = [];
  @Input() loading = false;
  
  private prevBillsLength = 0;
  
  @ViewChild('chartCanvas') chartCanvas!: ElementRef<HTMLCanvasElement>;
  
  private chart: Chart | null = null;
  
  ngAfterViewInit() {
    this.createChart();
  }
  
  ngOnChanges(changes: SimpleChanges) {
    // Check if bills have changed or if loading has finished
    const billsChanged = this.bills.length !== this.prevBillsLength;
    const loadingFinished = changes['loading'] && !this.loading;
    
    if (billsChanged || loadingFinished) {
      this.prevBillsLength = this.bills.length;
      this.updateChart();
    }
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
    
    if (this.bills.length === 0) {
      console.log('No data to display');
      this.clearChart();
      return;
    }
    
    // Process bills to create monthly consumption data for the last 3 years
    const monthlyData = this.processBillsForMonthlyData(this.bills);
    
    // Prepare the datasets for the last 3 years
    const yearData: { [year: string]: { consumption: number[] } } = {};
    
    // Process the data to group by year
    monthlyData.forEach(entry => {
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
  
  processBillsForMonthlyData(bills: ElectricBill[]) {
    // Process bills to create monthly consumption data for the last 3 years
    const data = [];
    const currentYear = new Date().getFullYear();
    const threeYearsAgo = currentYear - 2;
    
    // Group bills by year/month to calculate total consumption per month
    const monthlyTotals: { [key: string]: number } = {};
    
    bills.forEach(bill => {
      if (!bill.serviceYear || !bill.consumptionKwh) return;
      
      // Only process bills from the last 3 years
      if (bill.serviceYear >= threeYearsAgo && bill.serviceYear <= currentYear) {
        const month = bill.serviceMonth || 1; // Default to January if month is missing
        
        // Create a key for year/month combination
        const key = `${bill.serviceYear}-${month}`;
        
        if (!monthlyTotals[key]) {
          monthlyTotals[key] = 0;
        }
        
        // Add consumption to monthly total
        monthlyTotals[key] += bill.consumptionKwh;
      }
    });
    
    // Generate all months for the last 3 years with actual or zero consumption
    for (let year = threeYearsAgo; year <= currentYear; year++) {
      for (let month = 1; month <= 12; month++) {
        const key = `${year}-${month}`;
        const consumption = monthlyTotals[key] || 0;
        
        data.push({
          year: year,
          month: month,
          consumption: consumption
        });
      }
    }
    
    return data;
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