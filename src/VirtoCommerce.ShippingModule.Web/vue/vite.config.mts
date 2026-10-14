import path from "node:path";
import { getDynamicModuleConfiguration } from "@vc-shell/mf-module";

// Shipping methods and pickup locations of stores: a Module Federation remote of the Admin Shell host.
// Output: <module>/plugins/admin-shell/ (remoteEntry.js + public/plugin.json),
// served by the platform at /modules/$(VirtoCommerce.Shipping)/plugins/admin-shell/.
const base = "/modules/$(VirtoCommerce.Shipping)/plugins/admin-shell/";

export default getDynamicModuleConfiguration({
  appId: "admin-shell",
  moduleRoot: path.resolve(__dirname, ".."),
  entry: "./src/modules/index.ts",
  remoteName: "shipping_admin",
  base,
});
