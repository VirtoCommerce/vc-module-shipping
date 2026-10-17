<template>
  <VcBlade
    :title="$t('SHIPPING.PICKUP.TITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcDataTable
      v-model:search-value="keyword"
      v-model:selection="selection"
      selection-mode="multiple"
      :items="items"
      :total-count="totalCount"
      :pagination="pagination"
      :searchable="true"
      :state-key="'STORE_PICKUP_LOCATIONS'"
      :active-item-id="activeItemId"
      :empty-state="{ icon: 'lucide-map-pin', title: $t('SHIPPING.PICKUP.EMPTY') }"
      @row-click="(e: { data: PickupLocation }) => openDetails(e.data.id)"
      @pagination-click="pagination.goToPage"
    >
      <VcColumn
        id="name"
        :title="$t('SHIPPING.FIELDS.NAME')"
        always-visible
      />
      <VcColumn
        id="address"
        :title="$t('SHIPPING.PICKUP.ADDRESS')"
      >
        <template #body="{ data: p }">{{ addressLine(p.address) }}</template>
      </VcColumn>
      <VcColumn
        id="isActive"
        :title="$t('SHIPPING.FIELDS.STATUS')"
        always-visible
      >
        <template #body="{ data: p }">
          <div class="tw-inline-flex">
            <VcStatus :variant="p.isActive ? 'success' : 'info'">
              {{ $t(p.isActive ? "SHIPPING.ACTIVE" : "SHIPPING.INACTIVE") }}
            </VcStatus>
          </div>
        </template>
      </VcColumn>
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, useDataTablePagination, useFunctions, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcStatus } from "@vc-shell/framework/ui";
import { computed, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { api } from "../api";
import { addressLine, type PickupLocation } from "../pickup";

// Pickup locations of a store (the "Buy online, pick up in store" method); param = store id
defineBlade({
  url: "/shipping-pickup-locations",
  name: "ShippingPickupLocations",
});

const { t } = useI18n({ useScope: "global" });
const { param, openBlade, callParent, exposeToChildren } = useBlade();
const { showConfirmation } = usePopup();
const { debounce } = useFunctions();

const PAGE_SIZE = 20;
const items = ref<PickupLocation[]>([]);
const totalCount = ref(0);
const keyword = ref("");
const selection = ref<PickupLocation[]>([]);
const activeItemId = ref<string>();
const pagination = useDataTablePagination({ pageSize: PAGE_SIZE, totalCount });

const { loading, action: load } = useAsync(async () => {
  const result = await api.post<{ results?: PickupLocation[]; totalCount?: number }>("/api/shipping/pickup-locations/search", {
    storeId: param.value,
    keyword: keyword.value || undefined,
    skip: pagination.skip,
    take: PAGE_SIZE,
  });
  items.value = result.results ?? [];
  totalCount.value = result.totalCount ?? 0;
});

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("SHIPPING.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  { id: "add", title: t("SHIPPING.TOOLBAR.ADD"), icon: "lucide-plus", permissions: "shipping:create", clickHandler: () => openDetails() },
  {
    id: "delete",
    title: t("SHIPPING.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: "shipping:delete",
    disabled: computed(() => !selection.value.length),
    async clickHandler() {
      if (await showConfirmation(t("SHIPPING.ALERTS.DELETE_SELECTED", { count: selection.value.length }))) {
        await deleteSelected().catch(() => undefined); // useAsync shows the error
      }
    },
  },
]);

// the list is reloaded even when a delete fails: the others are gone on the server
const { action: deleteSelected } = useAsync(async () => {
  try {
    const results = await Promise.allSettled(
      selection.value.map((p) => api.delete(`/api/shipping/pickup-locations/${encodeURIComponent(param.value!)}/${p.id}`)),
    );
    const failed = results.find((x) => x.status === "rejected");
    if (failed) throw failed.reason;
  } finally {
    selection.value = [];
    await reload();
  }
});

function openDetails(id?: string) {
  activeItemId.value = id;
  openBlade({ name: "ShippingPickupLocationDetails", param: id, options: { storeId: param.value }, onClose: () => (activeItemId.value = undefined) });
}

async function reload() {
  await load();
  await callParent("reloadPickupCount").catch(() => undefined);
}

// off the first page, the page change loads
watch(keyword, debounce(() => (pagination.skip ? pagination.setPage(1) : load()), 400));
watch(() => pagination.skip, () => load());
onMounted(load);
exposeToChildren({ reload });
</script>
