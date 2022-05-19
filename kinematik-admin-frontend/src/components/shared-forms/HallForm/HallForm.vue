<template>
  <Fragment v-if="formData">
    <b-form-group label="Назва:">
      <b-input v-model="formData.title"></b-input>
    </b-form-group>
  </Fragment>
</template>

<script lang="ts">
import Vue from 'vue';
import { Fragment } from 'vue-fragment';

import HallFormData from '@/components/shared-forms/HallForm/HallFormData';

export default Vue.extend({
  name: 'HallForm',
  components: {
    Fragment,
  },
  props: {
    formData: Object as () => HallFormData | null,
  },
  model: {
    prop: 'formData',
  },
});
</script>

<style scoped lang="scss"></style>
