import type { SettingEntry } from "./components/SettingsEditor.vue";
import { legacyText } from "./utils";

// Shipping methods of a store (api/shipping). The server picks the concrete type from typeName: PUT the whole object back.
export interface ShippingMethod {
  id?: string | null;
  code?: string;
  name?: string;
  description?: string;
  typeName?: string;
  logoUrl?: string | null;
  isActive?: boolean;
  priority?: number;
  storeId?: string;
  taxType?: string;
  settings?: SettingEntry[];
}

export const URL = "/api/shipping";

// the "Buy online, pick up in store" method: it has pickup locations
export const PICKUP_TYPE = "BuyOnlinePickupInStoreShippingMethod";

// display name/description: the AngularJS texts per type ("shipping.labels.<typeName>.name"), else the method's own
export const methodName = (texts: Record<string, unknown>, m: ShippingMethod) =>
  legacyText(texts, "shipping", "labels", m.typeName ?? "", "name") ?? m.name ?? m.typeName ?? m.code ?? "";
export const methodDescription = (texts: Record<string, unknown>, m: ShippingMethod) =>
  legacyText(texts, "shipping", "labels", m.typeName ?? "", "description") ?? m.description ?? "";
