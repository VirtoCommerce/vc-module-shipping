<template>
  <VcBlade
    :title="isNew ? $t('SHIPPING.PICKUP.NEW_TITLE') : item.name ?? ''"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="60%"
  >
    <VcContainer>
      <VcForm class="shipping-form tw-space-y-4">
        <VcCard :header="$t('SHIPPING.GENERAL')">
          <div class="shipping-card">
            <VcRow class="tw-gap-4">
              <VcCol>
                <VcInput
                  v-model="item.name"
                  :label="$t('SHIPPING.FIELDS.NAME')"
                  required
                />
              </VcCol>
              <VcCol>
                <VcSwitch
                  :model-value="!!item.isActive"
                  :label="$t('SHIPPING.FIELDS.IS_ACTIVE')"
                  :hint="activeError"
                  @update:model-value="setActive"
                />
              </VcCol>
            </VcRow>
            <VcRow class="tw-gap-4">
              <VcCol>
                <VcInput
                  v-model="item.contactEmail"
                  :label="$t('SHIPPING.PICKUP.EMAIL')"
                  type="email"
                />
              </VcCol>
              <VcCol>
                <VcInput
                  v-model="item.contactPhone"
                  :label="$t('SHIPPING.PICKUP.PHONE')"
                />
              </VcCol>
            </VcRow>
            <VcRow class="tw-gap-4">
              <VcCol>
                <VcInput
                  v-model="item.geoLocation"
                  :label="$t('SHIPPING.PICKUP.GEO')"
                  placeholder="47.6062, -122.3321"
                  :error-message="geoInvalid ? $t('SHIPPING.PICKUP.GEO_ERROR') : undefined"
                />
              </VcCol>
              <VcCol>
                <VcInput
                  v-model="item.outerId"
                  :label="$t('SHIPPING.PICKUP.OUTER_ID')"
                />
              </VcCol>
            </VcRow>
            <VcRow class="tw-gap-4">
              <VcCol>
                <VcInput
                  :model-value="item.deliveryDays ?? ''"
                  :label="$t('SHIPPING.PICKUP.DELIVERY_DAYS')"
                  type="number"
                  @update:model-value="(v) => (item.deliveryDays = v === '' || v === null ? null : Number(v))"
                />
              </VcCol>
              <VcCol>
                <VcInput
                  :model-value="item.storageDays ?? ''"
                  :label="$t('SHIPPING.PICKUP.STORAGE_DAYS')"
                  type="number"
                  @update:model-value="(v) => (item.storageDays = v === '' || v === null ? null : Number(v))"
                />
              </VcCol>
            </VcRow>
            <template v-if="inventory">
              <VcRow class="tw-gap-4">
                <VcCol>
                  <VcSelect
                    v-model="item.fulfillmentCenterId"
                    :label="$t('SHIPPING.PICKUP.FULFILLMENT_CENTER')"
                    :options="searchFulfillmentCenters"
                    option-label="name"
                    searchable
                  />
                </VcCol>
                <VcCol>
                  <VcSelect
                    v-model="item.transferFulfillmentCenterIds"
                    :label="$t('SHIPPING.PICKUP.TRANSFER_CENTERS')"
                    :options="searchFulfillmentCenters"
                    option-label="name"
                    multiple
                    searchable
                  />
                </VcCol>
              </VcRow>
            </template>
            <VcTextarea
              v-model="item.description"
              :label="$t('SHIPPING.FIELDS.DESCRIPTION')"
            />
          </div>
        </VcCard>

        <VcCard :header="$t('SHIPPING.PICKUP.ADDRESS')">
          <div class="shipping-card">
            <VcRow class="tw-gap-4">
              <VcCol>
                <VcSelect
                  :model-value="address.countryCode"
                  :label="$t('SHIPPING.PICKUP.COUNTRY')"
                  :options="countries"
                  option-label="name"
                  searchable
                  required
                  @update:model-value="setCountry"
                />
              </VcCol>
              <VcCol>
                <VcSelect
                  v-if="regions.length"
                  :model-value="address.regionId"
                  :label="$t('SHIPPING.PICKUP.REGION')"
                  :options="regions"
                  option-label="name"
                  searchable
                  @update:model-value="setRegion"
                />
                <VcInput
                  v-else
                  v-model="address.regionName"
                  :label="$t('SHIPPING.PICKUP.REGION')"
                />
              </VcCol>
            </VcRow>
            <VcRow class="tw-gap-4">
              <VcCol>
                <VcInput
                  v-model="address.city"
                  :label="$t('SHIPPING.PICKUP.CITY')"
                  required
                />
              </VcCol>
              <VcCol>
                <VcInput
                  v-model="address.postalCode"
                  :label="$t('SHIPPING.PICKUP.POSTAL_CODE')"
                  required
                />
              </VcCol>
            </VcRow>
            <VcInput
              v-model="address.line1"
              :label="$t('SHIPPING.PICKUP.LINE1')"
            />
            <VcInput
              v-model="address.line2"
              :label="$t('SHIPPING.PICKUP.LINE2')"
            />
          </div>
        </VcCard>

        <VcCard :header="$t('SHIPPING.PICKUP.WORKING_HOURS')">
          <div class="shipping-card">
            <VcEditor
              v-model="workingHours"
              assets-folder="cms-content/image/assets"
            />
          </div>
        </VcCard>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, useBladeForm, useLoading, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import {
  VcBlade,
  VcCard,
  VcCol,
  VcContainer,
  VcEditor,
  VcForm,
  VcInput,
  VcRow,
  VcSelect,
  VcSwitch,
  VcTextarea,
} from "@vc-shell/framework/ui";
import { computed, inject, nextTick, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { api } from "../api";
import { addressValid, type PickupAddress, type PickupLocation } from "../pickup";

// not routable: the store comes only through options (the API needs it with the id), which a page reload doesn't restore
defineBlade({
  url: "/shipping-pickup-location",
  name: "ShippingPickupLocationDetails",
  routable: false,
});

const { t } = useI18n({ useScope: "global" });
const { param, options, callParent, closeSelf, replaceWith } = useBlade<{ storeId: string }>();
const { showConfirmation } = usePopup();

const isNew = computed(() => !param.value);
const storeId = computed(() => options.value?.storeId ?? "");
const item = ref<PickupLocation>({ id: "", storeId: storeId.value, isActive: false, address: {} });
const address = computed(() => item.value.address as PickupAddress);
// working hours are HTML (as the AngularJS HTML editor saved them): an empty paragraph makes the editor keep HTML
// instead of switching to Markdown, and is not an edit
const workingHours = computed({
  get: () => item.value.workingHours || "<p></p>",
  set: (v: string) => {
    if (v !== (item.value.workingHours ?? "") && !(v === "<p></p>" && !item.value.workingHours)) item.value.workingHours = v;
  },
});
const geoInvalid = computed(() => !!item.value.geoLocation && !/^-?\d+(\.\d+)?,\s*-?\d+(\.\d+)?$/.test(item.value.geoLocation));
const countries = ref<{ id: string; name: string }[]>([]);
const regions = ref<{ id: string; name: string }[]>([]);
// the fulfillment centers come from the Inventory module: shown only when it is installed (as the AngularJS blade did),
// as the Admin Shell knows it ($modules, vc-module-admin-shell docs/plugins.md "Using other modules"); hidden in a host
// without it
const modules = inject<{ isInstalled(moduleId: string): boolean } | undefined>("adminShell.modules", undefined);
const inventory = computed(() => modules?.isInstalled("VirtoCommerce.Inventory") ?? false);
const activeError = ref("");

const form = useBladeForm({
  data: item,
  canSaveOverride: computed(() => !!item.value.name && !geoInvalid.value),
  closeConfirmMessage: computed(() => t("SHIPPING.ALERTS.CLOSE_CONFIRMATION")),
});

async function setActive(value?: boolean) {
  item.value.isActive = !!value;
  // as in the AngularJS blade: only a location with a complete address can be active
  if (value && !addressValid(item.value.address)) {
    activeError.value = t("SHIPPING.PICKUP.ADDRESS_REQUIRED");
    setTimeout(() => (activeError.value = ""), 4000);
    await nextTick();
    item.value.isActive = false;
  }
}

function setCountry(code: unknown) {
  const country = countries.value.find((c) => c.id === code);
  item.value.address = { ...address.value, countryCode: country?.id, countryName: country?.name, regionId: undefined, regionName: undefined };
}

function setRegion(id: unknown) {
  const region = regions.value.find((r) => r.id === id);
  item.value.address = { ...address.value, regionId: region?.id, regionName: region?.name };
}

watch(
  () => address.value.countryCode,
  async (code) => {
    regions.value = code ? await api.get<{ id: string; name: string }[]>(`/api/platform/common/countries/${code}/regions`).catch(() => []) : [];
  },
);

async function searchFulfillmentCenters(keyword?: string, skip = 0, ids?: string[]) {
  return api.post<{ results?: unknown[]; totalCount?: number }>("/api/inventory/fulfillmentcenters/search", {
    keyword,
    objectIds: ids,
    skip,
    take: ids?.length || 20,
  });
}

const url = "/api/shipping/pickup-locations";

const { loading: fetching, action: load } = useAsync(async () => {
  const location = await api.get<PickupLocation>(`${url}/${encodeURIComponent(storeId.value)}/${param.value}`);
  item.value = { ...location, address: location.address ?? {} };
  form.setBaseline();
});

const { loading: saving, action: save } = useAsync(async () => {
  const location = { ...item.value, storeId: storeId.value };
  if (location.isActive && !addressValid(location.address)) location.isActive = false;
  if (isNew.value) {
    const created = await api.post<PickupLocation>(url, { ...location, id: undefined });
    form.setBaseline();
    await callParent("reload");
    await replaceWith({ name: "ShippingPickupLocationDetails", param: created.id, options: options.value });
  } else {
    await api.put(url, location);
    await load();
    await callParent("reload");
  }
});

const loading = useLoading(fetching, saving);

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    title: computed(() => t(isNew.value ? "SHIPPING.TOOLBAR.CREATE" : "SHIPPING.TOOLBAR.SAVE")),
    icon: "lucide-save",
    permissions: isNew.value ? "shipping:create" : "shipping:update",
    disabled: computed(() => !form.canSave.value),
    clickHandler: () => save(),
  },
  {
    id: "reset",
    title: t("SHIPPING.TOOLBAR.RESET"),
    icon: "lucide-undo-2",
    disabled: computed(() => !form.isModified.value),
    clickHandler: () => form.revert(),
  },
  {
    id: "delete",
    title: t("SHIPPING.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: "shipping:delete",
    isVisible: computed(() => !isNew.value),
    async clickHandler() {
      if (await showConfirmation(t("SHIPPING.ALERTS.DELETE_SELECTED", { count: 1 }))) {
        await api.delete(`${url}/${encodeURIComponent(storeId.value)}/${item.value.id}`);
        form.setBaseline();
        await callParent("reload");
        await closeSelf();
      }
    },
  },
]);

onMounted(async () => {
  const [list] = await Promise.all([
    api.get<{ id: string; name: string }[]>("/api/platform/common/countries"),
    isNew.value ? Promise.resolve() : load(),
  ]);
  countries.value = list;
  if (isNew.value) form.setBaseline();
});
</script>
