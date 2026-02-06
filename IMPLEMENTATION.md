# Service Address Selector Component Implementation

## Overview

This implementation adds a reusable service address selector component to allow users to select their service address from a dropdown. It integrates with the existing API to fetch user service addresses.

## Files Created

1. **Service Address Selector Component**
   - `src/app/components/service-address-selector/service-address-selector.ts` - Component logic
   - `src/app/components/service-address-selector/service-address-selector.html` - Component template
   - `src/app/components/service-address-selector/service-address-selector.css` - Component styling

2. **API Enhancement**
   - Enhanced `src/app/services/api.ts` with `getAddresses()` method

3. **Integration**
   - Updated `src/app/pages/billing-info/billing-info.ts` to use the component
   - Updated `src/app/pages/billing-info/billing-info.html` to include the component

## Key Features

- Fetches service addresses from `/api/users/address` endpoint
- Dropdown selection for choosing addresses
- Automatic selection of primary address or first address if none selected
- Error handling for API failures
- Loading states during data fetch
- Responsive design with clean styling

## Usage

The component can be used in multiple places in the application by simply importing and using:

```html
<app-service-address-selector 
  [selectedAddressId]="addressId"
  [loading]="addressesLoading" 
  (addressSelected)="onAddressSelected($event)">
</app-service-address-selector>
```

The selected address ID is emitted through the `addressSelected` event and can be handled by the parent component.