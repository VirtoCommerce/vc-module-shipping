# Shipping Module

[![CI status](https://github.com/VirtoCommerce/vc-module-shipping/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-shipping/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-shipping&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-shipping) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-shipping&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-shipping) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-shipping&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-shipping) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-shipping&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-shipping)

The Shipping Module provides the ability to extend the shipping provider list with custom providers and offers a UI and API for managing these shipping providers across stores. It ships with two built-in shipping methods — **Fixed Rate** and **Buy Online, Pickup In Store (BOPIS)** — and an extensibility point for registering additional methods from other modules.

## Overview

Shipping methods are registered in code and then made available for configuration per store from the admin UI. Each method exposes its own set of settings (rates, surcharges, credentials, etc.) which are edited from the store's shipping tab. At checkout/quotation time the platform asks every active method for a rate, and the calculated rates are returned to the storefront.

In addition to rate calculation, the module manages **pickup locations** used by BOPIS: dedicated entities with addresses, business hours, geo-coordinates and optional Google Maps integration. Pickup locations are indexed via the Search module so they can be looked up quickly on the storefront and in the admin UI.

Key concepts:

* **Shipping method** — a pluggable provider registered via DI that calculates shipping rates for a shipment.
* **Pickup location** — a physical point (store, locker, warehouse) where a customer can pick up an order under the BOPIS method.
* **Store binding** — each shipping method is activated per store, with its own settings and priority.

## Features

* Register custom shipping methods in code and expose them to the admin UI.
* Manage the full list of shipping methods from the admin UI, with priority and per-store activation.
* Edit per-method settings directly from the store's shipping tab.
* Built-in **Fixed Rate** shipping method with configurable Ground and Air rates.
* Built-in **BOPIS** (Buy Online, Pickup In Store) shipping method with full pickup-location management.
* **Pickup locations CRUD**: create, update, delete and search locations with address, contacts and geo-coordinates.
* **Google Maps integration** for visualizing and picking pickup-location coordinates (optional, requires API key).
* **Indexed search** for pickup locations via the Search module, with event-based re-indexation out of the box.
* Permission-scoped access (`shipping:read`, `shipping:create`, `shipping:update`, `shipping:delete`) and a `SelectedStoreScope` to limit shipping management to specific stores.
* Public REST API to work with shipping methods, pickup locations and indexed search.

## Settings

The module registers the following settings. They can be edited from **Settings → Shipping** in the admin UI, or overridden via `appsettings.json` like any other platform setting.

### BOPIS (pickup locations)

| Setting | Type | Default | Description |
| --- | --- | --- | --- |
| `Shipping.Bopis.GoogleMaps.Enabled` | Boolean | `false` | Enable Google Maps in the pickup-location editor to pick/visualize coordinates. |
| `Shipping.Bopis.GoogleMaps.ApiKey` | Short text | *(empty)* | Google Maps JavaScript API key used when the setting above is enabled. Exposed to the frontend (`IsPublic`). |
| `Shipping.Bopis.Search.EventBasedIndexation.Enabled` | Boolean | `true` | Re-index a pickup location as soon as it is created/updated/deleted, in addition to the scheduled indexing job. |
| `VirtoCommerce.Search.IndexingJobs.IndexationDate.PickupLocation` | DateTime | *(unset)* | Checkpoint used by the pickup-location indexing job to track the last processed change. |

### Fixed Rate shipping method

| Setting | Type | Default | Description |
| --- | --- | --- | --- |
| `VirtoCommerce.Shipping.FixedRateShippingMethod.Ground.Rate` | Decimal | `0.00` | Flat rate applied when the Fixed Rate method quotes *Ground* delivery. |
| `VirtoCommerce.Shipping.FixedRateShippingMethod.Air.Rate` | Decimal | `0.00` | Flat rate applied when the Fixed Rate method quotes *Air* delivery. |

## Documentation

* [Shipping module user documentation](https://docs.virtocommerce.org/platform/user-guide/shipping/overview/)
* [REST API](https://virtostart-demo-admin.govirto.com/docs/index.html?urls.primaryName=VirtoCommerce.Shipping)
* [View on GitHub](https://github.com/VirtoCommerce/vc-module-shipping)

## References

* [Deployment](https://docs.virtocommerce.org/platform/developer-guide/Tutorials-and-How-tos/Tutorials/deploy-module-from-source-code/)
* [Installation](https://docs.virtocommerce.org/platform/user-guide/modules-installation/)
* [Home](https://virtocommerce.com)
* [Community](https://www.virtocommerce.org)
* [Download latest release](https://github.com/VirtoCommerce/vc-module-shipping/releases/latest)

## License

Copyright (c) Virto Solutions LTD. All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
