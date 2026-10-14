import * as blades from "./pages";
import * as locales from "./locales";
import { defineAppModule, registerExternalWidget } from "@vc-shell/framework";
import { markRaw } from "vue";
import StoreShippingMethodsWidget from "./widgets/StoreShippingMethodsWidget.vue";

// The AngularJS module's shipping methods widget on the store blade ("storeDetail")
registerExternalWidget({
  id: "ShippingStoreMethods",
  title: "Shipping methods",
  component: markRaw(StoreShippingMethodsWidget),
  targetBlades: ["StoreDetails"],
  // not on a store that isn't saved yet
  isVisible: (blade) => !!blade?.param,
});

export default defineAppModule({
  blades,
  locales,
});
