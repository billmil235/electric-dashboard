export interface Forecast {
  addressId: string;
  forecastYear: number;
  forecastMonth: number;
  predictedAmount: number;
  algorithmUsed: string;
  confidence: number;
}
