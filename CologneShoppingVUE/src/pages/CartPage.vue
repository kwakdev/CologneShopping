<template>
  <div class="text-center">
    <div class="text-h4 q-mt-md text-primary">Order Contents</div>

    <div class="text-h6 text-positive">{{ state.status }}</div>
    <q-avatar class="q-mb-md" size="xl" square>
      <img :src="`/img/shopping-cart.png`" />
    </q-avatar>
  </div>

  <q-item class="order-header">
    <q-item-section class="col-2 q-ml-sm text-h6 text-primary">
      QTY
    </q-item-section>
    <q-item-section class="q-ml-sm text-h6 text-primary text-left">
      NAME
    </q-item-section>
    <q-item-section class="q-ml-sm text-h6 text-primary text-left">
      PRICE
    </q-item-section>
    <q-item-section class="q-ml-sm text-h6 text-primary text-left">
      MODIFIED PRICE
    </q-item-section>
  </q-item>

  <q-scroll-area style="height: 55vh">
    <q-card class="q-ma-md">
      <q-list separator>
        <q-item v-for="item in state.order" :key="item.id">
          <q-item-section>
            {{ item.qty }}
          </q-item-section>
          <q-item-section class="text-left">
            {{ item.item.productName }}
          </q-item-section>
          <q-item-section class="text-left">
            {{ formatCurrency(item.item.msrp) }}
          </q-item-section>
          <q-item-section class="text-left">
            {{ formatCurrency(item.qty * item.item.msrp) }}
          </q-item-section>
        </q-item>

        <!-- Conditionally render totals if there are items in the cart -->
        <template v-if="state.order.length > 0">
          <q-item>
            <q-item-section class="text-left text-bold">
              Subtotal
            </q-item-section>
            <q-item-section class="text-left">
              {{ formatCurrency(state.subtotal) }}
            </q-item-section>
          </q-item>
          <q-item>
            <q-item-section class="text-left text-bold"> Tax </q-item-section>
            <q-item-section class="text-left">
              {{ formatCurrency(state.tax) }}
            </q-item-section>
          </q-item>
          <q-item>
            <q-item-section class="text-left text-bold text-secondary">
              Total
            </q-item-section>
            <q-item-section class="text-left text-secondary">
              {{ formatCurrency(state.total) }}
            </q-item-section>
          </q-item>
        </template>
      </q-list>
    </q-card>
  </q-scroll-area>

  <div class="row q-pa-md" style="margin-top: 10px">
    <q-btn
      color="primary"
      label="Clear order"
      @click="clearorder"
      style="max-width: 25vw; margin-left: 4px"
    />
    <q-btn
      class="q-mr-sm"
      color="primary"
      style="max-width: 25vw; margin-left: 4px"
      label="Save order"
      :disable="state.order.length < 1"
      @click="saveorder"
    />
  </div>
</template>

<script>
import { reactive, onMounted } from "vue";
import { useRouter } from "vue-router";
import { poster } from "../utils/apiutil";
import { formatCurrency } from "../utils/formatutils";

export default {
  setup() {
    const router = useRouter();
    const TAX_RATE = 0.15;

    let state = reactive({
      status: "Loading order contents...",
      order: [],
      subtotal: 0,
      tax: 0,
      total: 0,
    });

    onMounted(() => {
      loadorder();
    });

    const loadorder = () => {
      if (sessionStorage.getItem("order")) {
        state.order = JSON.parse(sessionStorage.getItem("order"));
        state.status = "Order loaded";
        calculateTotals();
      } else {
        state.status = "Order is empty";
      }
    };

    const calculateTotals = () => {
      state.subtotal = 0;
      state.order.forEach((orderItem) => {
        state.subtotal += orderItem.qty * orderItem.item.msrp;
      });
      state.tax = state.subtotal * TAX_RATE;
      state.total = state.subtotal + state.tax;
    };

    const saveorder = async () => {
      let customer = JSON.parse(sessionStorage.getItem("customer"));
      let order = JSON.parse(sessionStorage.getItem("order"));
      try {
        state.status = "Sending order info to server";
        let orderHelper = { email: customer.email, selections: order };
        let payload = await poster("Order", orderHelper);
        if (payload.indexOf("not") > 0) {
          state.status = payload;
        } else {
          clearorder();
          state.status = payload;
        }
      } catch (err) {
        console.log(err);
        state.status = `Error adding order: ${err}`;
      }
    };

    const clearorder = () => {
      sessionStorage.removeItem("order");
      state.order = [];
      state.status = "Order cleared";
      state.subtotal = 0;
      state.tax = 0;
      state.total = 0;
    };

    return {
      state,
      clearorder,
      formatCurrency,
      saveorder,
      router,
    };
  },
};
</script>

<style scoped>
.text-center {
  text-align: center;
}
.q-mt-md {
  margin-top: 1rem;
}
.q-mt-lg {
  margin-top: 2rem;
}
.q-ma-md {
  margin: 1rem;
}
.q-pa-md {
  padding: 1rem;
}
</style>
