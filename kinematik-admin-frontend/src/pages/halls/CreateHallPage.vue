<template>
  <main>
    <h1>Створити залу</h1>

    <form class="mt-5">
      <HallForm v-model="hallFormData"></HallForm>

      <div class="d-flex flex-row justify-content-end mt-4 gap-2">
        <b-button variant="success" v-on:click="addHall()">
          <i class="fa-solid fa-floppy-disk"></i> Зберегти
        </b-button>
        <b-button variant="outline-success" v-on:click="addHall()">
          <i class="fa-solid fa-check"></i> Зберегти та додати ще
        </b-button>
      </div>
    </form>
  </main>
</template>

<script lang="ts">
import Vue from 'vue';
import axios from 'axios';

import HallForm from '@/components/shared-forms/HallForm/HallForm.vue';
import HallFormData from '@/components/shared-forms/HallForm/HallFormData';

export default Vue.extend({
  name: 'CreateHallPage',
  components: {
    HallForm,
  },
  data() {
    return {
      hallFormData: {
        title: '',
        layoutRows: [],
      } as HallFormData,
    };
  },
  methods: {
    addHall() {
      axios.post('/halls', {
        title: this.hallFormData.title,
        layoutItems: this.hallFormData.layoutRows.flat(),
      });
    },
  },
});
</script>

<style scoped lang="scss"></style>
