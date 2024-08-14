<template>
  <div class="text-h4 text-center q-mt-md q-mb-md text-primary">
    Logging out...
  </div>
</template>

<script>
import { onMounted } from "vue";
import { useRouter } from "vue-router";

export default {
  setup() {
    const router = useRouter();

    onMounted(() => {
      sessionStorage.clear();
      router.push({ path: "/" });
    });

    return {};
  },
};
</script>
