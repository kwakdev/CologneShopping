<template>
  <div class="text-center">
    <div class="text-h2 q-mt-lg">Brands</div>
    <q-avatar class="q-mb-md" size="xl" square>
      <img :src="`/img/yourgraphicname.png`" />
    </q-avatar>
    <div class="status q-mt-md text-subtitle2 text-negative" text-color="red">
      {{ state.status }}
    </div>
  </div>

  <p></p>
  <ul>
    <q-select
      class="q-mt-lg q-ml-lg"
      v-if="state.categories.length > 0"
      style="width: 50vw; margin-bottom: 4vh; background-color: #fff"
      :option-value="'id'"
      :option-label="'name'"
      :options="state.categories"
      label="Select a Brand"
      v-model="state.selectedbrandId"
      @update:model-value="getCologneitems"
      emit-value
    />
    <div
      class="text-h6 text-bold text-center text-primary"
      v-if="state.cologneeitems.length > 0"
    >
      {{ selectedbrand.name }} ITEMS
    </div>
    <q-scroll-area style="height: 55vh">
      <q-card class="q-ma-md">
        <q-list separator>
          <q-item
            clickable
            v-for="item in state.cologneeitems"
            :key="item.id"
            @click="selectMenuItem(item.id)"
          >
            <q-item-section avatar>
              <q-avatar style="height: 125px; width: 90px" square>
                <img
                  :src="`/img/${encodeURIComponent(item.graphicName)}.jpg`"
                  alt="Product Image"
                />
              </q-avatar>
            </q-item-section>
            <q-item-section class="text-left">
              {{ item.productName }}
            </q-item-section>
          </q-item>
        </q-list>
      </q-card>
    </q-scroll-area>
  </ul>

  <q-dialog
    v-model="state.itemSelected"
    transition-show="rotate"
    transition-hide="rotate"
  >
    <q-card>
      <q-card-actions align="right">
        <q-btn flat label="X" color="primary" v-close-popup class="text-h5" />
      </q-card-actions>
      <q-card-section>
        <div class="text-center">
          <img
            :src="`/img/${encodeURIComponent(
              state.selectedMenuItem.graphicName
            )}.jpg`"
            alt="Product Image"
            style="width: 100%; max-width: 300px"
          />
        </div>
      </q-card-section>
      <q-card-section>
        <div class="text-subtitle2 text-center">
          {{ state.selectedMenuItem.productName }} -
          {{ formatCurrency(state.selectedMenuItem.msrp) }}
        </div>
      </q-card-section>
      <div class="text-center">
        <q-card-section class="text-center text-positive">
          {{ state.dialogStatus }}
        </q-card-section>

        <q-card-section>
          <div class="row">
            <q-input
              v-model.number="state.qty"
              type="number"
              filled
              placeholder="qty"
              hint="Qty"
              dense
              style="max-width: 12vw"
            />
          </div>
        </q-card-section>
        <q-card-section>
          <q-chip icon="bookmark" color="primary" text-color="white">
            description
            <q-tooltip
              transition-show="flip-right"
              transition-hide="flip-left"
              text-color="white"
            >
              {{ state.selectedMenuItem.description }}
            </q-tooltip>
          </q-chip>
        </q-card-section>
        <q-card-section>
          <q-btn
            color="primary"
            label="Add To Cart"
            :disable="state.qty < 0"
            @click="addToorder"
            style="max-width: 25vw; margin-left: 3vw"
          />
          <q-btn
            color="secondary"
            label="View order"
            @click="vieworder()"
            style="max-width: 25vw; margin-left: 1vw"
          />
        </q-card-section>
      </div>
    </q-card>
  </q-dialog>
</template>

<script>
import { reactive, onMounted, computed } from "vue";
import { fetcher } from "../utils/apiutil";
import { formatCurrency } from "../utils/formatutils";

import { useRouter } from "vue-router";

export default {
  setup() {
    const router = useRouter();
    onMounted(() => {
      loadCategories();
    });

    let state = reactive({
      status: "",
      categories: [],
      cologneeitems: [],
      selectedbrandId: "",
      selectedMenuItem: {},
      dialogStatus: "",
      itemSelected: false,
      order: [],
      qty: 0,
    });

    const selectedbrand = computed(() => {
      return (
        state.categories.find((brand) => brand.id === state.selectedbrandId) ||
        {}
      );
    });

    const loadCategories = async () => {
      try {
        state.status = "Loading...";
        state.categories = await fetcher("brand");
        state.status = "Brands loaded.";
      } catch (error) {
        state.status = "Error loading categories.";
        console.error(error);
      }
    };

    const getCologneitems = async () => {
      if (!state.selectedbrandId) {
        console.warn("Selected brand ID is undefined or empty.");
        return;
      }

      try {
        console.log(
          `Fetching cologne items for brand ID: ${state.selectedbrandId}`
        );
        state.status = `Finding menu items for brand ${selectedbrand.value.name}...`;
        state.cologneeitems = await fetcher(
          `CologneItem/${state.selectedbrandId}`
        );
        state.status = `Loaded ${state.cologneeitems.length} menu items for ${selectedbrand.value.name}`;
      } catch (err) {
        console.log(err);
        state.status = `Error has occurred: ${err.message}`;
      }
    };

    const selectMenuItem = (menuitemid) => {
      try {
        state.selectedMenuItem = state.cologneeitems.find(
          (item) => item.id === menuitemid
        );
        state.itemSelected = true;
        state.dialogStatus = "";
        if (sessionStorage.getItem("order")) {
          state.order = JSON.parse(sessionStorage.getItem("order"));
        }
      } catch (err) {
        console.log(err);
        state.status = `Error has occurred: ${err.message}`;
      }
    };

    const addToorder = () => {
      let index = -1;
      if (state.order.length > 0) {
        index = state.order.findIndex(
          (item) => item.id === state.selectedMenuItem.id
        );
      }
      if (state.qty > 0) {
        index === -1
          ? state.order.push({
              id: state.selectedMenuItem.id,
              qty: state.qty,
              item: state.selectedMenuItem,
            })
          : (state.order[index] = {
              id: state.selectedMenuItem.id,
              qty: state.qty,
              item: state.selectedMenuItem,
            });
        state.dialogStatus = `${state.qty} item(s) added`;
      } else {
        index === -1 ? null : state.order.splice(index, 1);
        state.dialogStatus = `item(s) removed`;
      }
      sessionStorage.setItem("order", JSON.stringify(state.order));
      state.qty = 0;
    };
    const vieworder = () => {
      router.push("/cart");
    };
    return {
      state,

      selectedbrand,
      loadCategories,
      getCologneitems,
      selectMenuItem,
      addToorder,
      vieworder,

      formatCurrency,
    };
  },
};
</script>
