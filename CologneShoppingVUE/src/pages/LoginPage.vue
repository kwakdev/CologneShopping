<template>
  <div class="text-h4 text-center q-mt-md q-mb-md text-primary">Login</div>
  <div class="text-center">
    <q-avatar class="q-mb-md" size="xl" square>
      <img :src="`/img/yourgraphicname.png`" />
    </q-avatar>
  </div>
  <div class="text-title2 text-center text-positive text-bold q-mt-sm">
    {{ state.status }}
  </div>
  <q-card class="q-ma-md q-pa-md">
    <q-form
      ref="myForm"
      class="q-gutter-md"
      greedy
      @submit="login"
      @reset="resetFields"
    >
      <q-input
        outlined
        placeholder="Email"
        id="email"
        v-model="state.email"
        :rules="[isRequired, isValidEmail]"
      />
      <q-input
        outlined
        placeholder="Password"
        type="password"
        id="password"
        v-model="state.password"
        :rules="[isRequired]"
      />
      <q-btn label="Login" type="submit" />
      <q-btn label="Reset" type="reset" />
    </q-form>
  </q-card>
</template>

<script>
import { reactive } from "vue";
import { poster } from "../utils/apiutil"; // Adjust the path as needed
import { useRouter, useRoute } from "vue-router";

export default {
  setup() {
    const router = useRouter();
    const route = useRoute();
    let state = reactive({
      status: "",
      email: "",
      password: "",
    });

    const isRequired = (val) => {
      return !!val || "Field is required";
    };

    const isValidEmail = (val) => {
      const emailPattern =
        /^(?=[a-zA-Z0-9@._%+-]{6,254}$)[a-zA-Z0-9._%+-]{1,64}@(?:[a-zA-Z0-9-]{1,63}\.){1,8}[a-zA-Z]{2,63}$/;
      return emailPattern.test(val) || "Invalid email";
    };

    const login = async () => {
      state.status = "Logging in...";
      let custHelper = {
        email: state.email,
        password: state.password,
      };
      try {
        let payload = await poster("customer/login", custHelper);
        if (
          payload.token &&
          !payload.token.includes("invalid") &&
          !payload.token.includes("no such customer - login failed")
        ) {
          sessionStorage.setItem("customer", JSON.stringify(payload));
          state.status = "Login successful";
          route.query.nextUrl
            ? router.push({ path: route.query.nextUrl })
            : router.push({ path: "/" });
        } else {
          state.status = payload.token || "Login failed";
        }
      } catch (err) {
        state.status = err.message;
      }
    };

    const resetFields = () => {
      state.email = "";
      state.password = "";
      state.status = "";
    };

    return {
      state,
      login,
      isRequired,
      isValidEmail,
      resetFields,
    };
  },
};
</script>
