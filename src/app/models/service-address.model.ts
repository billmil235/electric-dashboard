export interface ServiceAddress {
  addressId: string;
  addressName: string;
  addressLine1: string;
  addressLine2: string | null;
  city: string;
  state: string;
  zipCode: string;
  country: string | null;
  isCommercial: boolean;
}