<template>
  <main>
    <h1>Зали</h1>

    <div class="d-flex flex-row">
      <b-button href="/halls/create" variant="success" class="ms-auto">
        <i class="fa-solid fa-plus"></i> Створити залу
      </b-button>
    </div>

    <b-table :items="halls" :fields="fields" hover striped class="mt-4">
      <template v-slot:cell(edit)="data">
        <b-button :href="`/halls/${data.item.id}`" variant="primary">
          <i class="fa-solid fa-pen"></i>
        </b-button>
      </template>

      <template v-slot:cell(remove)="data">
        <b-button v-on:click="removeHall(data.item.id)" variant="danger">
          <i class="fa-solid fa-trash"></i>
        </b-button>
      </template>
    </b-table>
  </main>
</template>

<script lang="ts">
import Vue from 'vue';
import axios from 'axios';

export default Vue.extend({
  name: 'ListHallsPage',
  data() {
    return {
      halls: [],
      fields: [
        {
          key: 'id',
          label: 'ID',
          class: 'text-center',
        },
        {
          key: 'title',
          label: 'Назва',
        },
        {
          key: 'edit',
          label: '',
          class: 'action-button',
        },
        {
          key: 'remove',
          label: '',
          class: 'action-button',
        },
      ],
    };
  },
  async mounted() {
    this.halls = await this.getHallsList();
  },
  methods: {
    async getHallsList() {
      const response = await axios.get('/halls/');

      return response.data.halls;
    },
    async refreshHallsList() {
      this.halls = await this.getHallsList();
    },
    async removeHall(hallID: number) {
      await axios.delete(`/halls/${hallID}`);
      await this.refreshHallsList();
    },
  },
});
</script>

<style lang="scss">
.action-button {
  width: 100px;
}
</style>
