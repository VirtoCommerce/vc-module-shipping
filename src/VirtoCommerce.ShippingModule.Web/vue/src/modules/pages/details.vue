<template>
  <VcBlade
    :title="name"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcContainer>
      <VcForm class="shipping-form tw-space-y-4">
        <VcCard :header="$t('SHIPPING.GENERAL')">
          <div class="shipping-card">
            <VcRow class="tw-gap-4">
              <VcCol>
                <VcField
                  :label="$t('SHIPPING.FIELDS.NAME')"
                  :model-value="name"
                />
              </VcCol>
              <VcCol>
                <VcField
                  :label="$t('SHIPPING.FIELDS.CODE')"
                  :model-value="item.code"
                  copyable
                />
              </VcCol>
            </VcRow>
            <VcRow class="tw-gap-4">
              <VcCol>
                <VcSwitch
                  v-model="item.isActive"
                  :label="$t('SHIPPING.FIELDS.IS_ACTIVE')"
                />
              </VcCol>
              <VcCol>
                <VcInput
                  :model-value="item.priority ?? 0"
                  :label="$t('SHIPPING.FIELDS.PRIORITY')"
                  type="number"
                  @update:model-value="(v) => (item.priority = Number(v) || 0)"
                />
              </VcCol>
            </VcRow>
            <VcSelect
              v-model="item.taxType"
              :label="$t('SHIPPING.FIELDS.TAX_TYPE')"
              :options="taxTypes"
            />
            <VcTextarea
              v-model="item.description"
              :label="$t('SHIPPING.FIELDS.DESCRIPTION')"
            />
            <ImageField
              v-model="item.logoUrl"
              :label="$t('SHIPPING.FIELDS.LOGO')"
              folder="/shipping-methods"
            />
          </div>
        </VcCard>

        <SettingsEditor
          v-if="item.settings?.length"
          :settings="item.settings"
        />
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, useBladeForm, useLoading, useBladeWidgets, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcCard, VcCol, VcContainer, VcField, VcForm, VcInput, VcRow, VcSelect, VcSwitch, VcTextarea } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { api, toOptions, type Option } from "../api";
import ImageField from "../components/ImageField.vue";
import SettingsEditor from "../components/SettingsEditor.vue";
import { PICKUP_TYPE, URL, methodName, type ShippingMethod } from "../methods";
import { loadLegacyTexts } from "../utils";

// not routable: the method and its store come only through options, which a page reload doesn't restore
defineBlade({
  url: "/shipping-method",
  name: "ShippingMethodDetails",
  routable: false,
});

const { t } = useI18n({ useScope: "global" });
const { options, callParent, openBlade, exposeToChildren } = useBlade<{ method?: ShippingMethod; storeId: string }>();

const item = ref<ShippingMethod>(JSON.parse(JSON.stringify(options.value?.method ?? {})));
const texts = ref<Record<string, unknown>>({});
const taxTypes = ref<Option[]>([]);
const name = computed(() => methodName(texts.value, item.value));

const form = useBladeForm({
  data: item,
  closeConfirmMessage: computed(() => t("SHIPPING.ALERTS.CLOSE_CONFIRMATION")),
});

const { loading: fetching, action: load } = useAsync(async () => {
  // a transient method (never saved for this store) has no id: it is created by the first save
  if (item.value.id) item.value = await api.get<ShippingMethod>(`${URL}/${item.value.id}`);
  form.setBaseline();
});

const { loading: saving, action: save } = useAsync(async () => {
  const saved = await api.put<ShippingMethod>(URL, { ...item.value, storeId: options.value?.storeId });
  item.value.id = saved?.id ?? item.value.id;
  await load();
  await callParent("reload");
});

const loading = useLoading(fetching, saving);

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    title: t("SHIPPING.TOOLBAR.SAVE"),
    icon: "lucide-save",
    permissions: "store:update",
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
]);

// "Buy online, pick up in store": its pickup locations (the AngularJS widget on the shipping method blade)
const isPickup = computed(() => item.value.typeName === PICKUP_TYPE);
const pickupCount = ref<number | string>("");
async function loadPickupCount() {
  if (!isPickup.value) return;
  const result = await api.post<{ totalCount?: number }>("/api/shipping/pickup-locations/search", { storeId: options.value?.storeId, take: 0 });
  pickupCount.value = result.totalCount ?? 0;
}
useBladeWidgets([
  {
    id: "ShippingPickupLocations",
    icon: "lucide-map-pin",
    title: "SHIPPING.PICKUP.TITLE",
    badge: pickupCount,
    isVisible: isPickup,
    onClick: () => openBlade({ name: "ShippingPickupLocations", param: options.value?.storeId }),
  },
]);
exposeToChildren({ reloadPickupCount: loadPickupCount });

onMounted(() =>
  Promise.all([
    loadLegacyTexts().then((x) => (texts.value = x)),
    // the pickup widget is for the method's type, known once it is loaded
    load().then(loadPickupCount),
    // as the AngularJS blade's settings.getValues: needs no setting permission; a refused lookup leaves the list empty
    api
      .get<unknown[]>("/api/platform/settings/values/VirtoCommerce.Core.General.TaxTypes")
      .catch(() => [])
      .then((x) => (taxTypes.value = toOptions(x))),
  ]),
);
</script>

<style lang="scss">
.shipping-form {
  padding: 20px 22px 36px;
}

.shipping-card {
  display: flex;
  flex-direction: column;
  gap: 16px;
  padding: 16px;
}
</style>
