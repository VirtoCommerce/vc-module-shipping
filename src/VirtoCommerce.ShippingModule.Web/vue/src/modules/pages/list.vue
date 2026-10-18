<template>
  <VcBlade
    :title="$t('SHIPPING.TITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcDataTable
      :items="rows"
      :state-key="'SHIPPING_METHODS'"
      :active-item-id="activeItemId"
      :show-all-columns="false"
      :empty-state="{ icon: 'lucide-truck', title: $t('SHIPPING.EMPTY') }"
      @row-click="(e: { data: Row }) => openDetails(e.data)"
    >
      <VcColumn
        id="logoUrl"
        :title="$t('SHIPPING.FIELDS.LOGO')"
        type="image"
        width="72px"
        always-visible
      />
      <VcColumn
        id="displayName"
        :title="$t('SHIPPING.FIELDS.NAME')"
        always-visible
      >
        <template #body="{ data: m }">
          <div class="shipping-methods__name">{{ m.displayName }}</div>
          <div
            v-if="m.displayDescription"
            class="shipping-methods__description"
          >
            {{ m.displayDescription }}
          </div>
        </template>
      </VcColumn>
      <VcColumn
        id="code"
        :title="$t('SHIPPING.FIELDS.CODE')"
      />
      <VcColumn
        id="priority"
        :title="$t('SHIPPING.FIELDS.PRIORITY')"
        width="90px"
      />
      <VcColumn
        id="isActive"
        :title="$t('SHIPPING.FIELDS.STATUS')"
        always-visible
      >
        <template #body="{ data: m }">
          <div class="tw-inline-flex">
            <VcStatus :variant="m.isActive ? 'success' : 'info'">
              {{ $t(m.isActive ? "SHIPPING.ACTIVE" : "SHIPPING.INACTIVE") }}
            </VcStatus>
          </div>
        </template>
      </VcColumn>
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcStatus } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { api } from "../api";
import { URL, methodDescription, methodName, type ShippingMethod } from "../methods";
import { useLegacyTexts } from "../utils";

// The shipping methods of a store (param = store id)
defineBlade({
  url: "/shipping-methods",
  name: "ShippingMethods",
});

type Row = ShippingMethod & { id: string; displayName: string; displayDescription: string };

const { t } = useI18n({ useScope: "global" });
const { param, openBlade, callParent, exposeToChildren } = useBlade();

const methods = ref<ShippingMethod[]>([]);
const texts = useLegacyTexts();
const activeItemId = ref<string>();

// transient methods (registered, never saved for this store) have no id yet: the code identifies them
const rows = computed<Row[]>(() =>
  methods.value
    .map((m) => ({
      ...m,
      id: (m.id ?? m.code)!,
      displayName: methodName(texts.value, m),
      displayDescription: methodDescription(texts.value, m),
    }))
    .sort((a, b) => (a.priority ?? 0) - (b.priority ?? 0) || a.displayName.localeCompare(b.displayName)),
);

const { loading, action: load } = useAsync(async () => {
  const result = await api.post<{ results?: ShippingMethod[] }>(`${URL}/search`, { storeId: param.value, take: 100 });
  methods.value = result.results ?? [];
});

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("SHIPPING.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
]);

function openDetails(row: Row) {
  activeItemId.value = row.id;
  openBlade({
    name: "ShippingMethodDetails",
    param: row.id,
    options: { method: methods.value.find((m) => (m.id ?? m.code) === row.id), storeId: param.value },
    onClose: () => (activeItemId.value = undefined),
  });
}

onMounted(load);

exposeToChildren({
  async reload() {
    await load();
    await callParent("reloadWidgets").catch(() => undefined); // the store blade's widget badges
  },
});
</script>

<style lang="scss">
.shipping-methods__name {
  color: var(--neutrals-900);
}
.shipping-methods__description {
  color: var(--neutrals-500);
  font-size: 12px;
}
</style>
