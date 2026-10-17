// Pickup locations of vc-module-shipping (api/shipping/pickup-locations)
export interface PickupAddress {
  countryCode?: string;
  countryName?: string;
  regionId?: string;
  regionName?: string;
  city?: string;
  postalCode?: string;
  line1?: string;
  line2?: string;
  firstName?: string;
  lastName?: string;
}

export interface PickupLocation {
  id: string;
  storeId?: string;
  isActive?: boolean;
  name?: string;
  description?: string;
  fulfillmentCenterId?: string;
  transferFulfillmentCenterIds?: string[];
  contactPhone?: string;
  contactEmail?: string;
  deliveryDays?: number | null;
  storageDays?: number | null;
  workingHours?: string;
  geoLocation?: string;
  outerId?: string;
  address?: PickupAddress;
}

// the AngularJS rule: a location can be active only with a country, city and postal code
export const addressValid = (a?: PickupAddress) => !!(a?.countryName && a.city && a.postalCode);

export const addressLine = (a?: PickupAddress) =>
  [a?.line1, a?.city, a?.regionName, a?.postalCode, a?.countryName].filter(Boolean).join(", ");
