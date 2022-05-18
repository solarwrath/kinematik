<template>
  <main>
    <h1>Фільми</h1>

    <div class="d-flex flex-row">
      <b-button href="/films/create" variant="success" class="ms-auto">
        <i class="fa-solid fa-plus"></i> Додати фільм до афіші
      </b-button>
    </div>

    <b-table :items="films" :fields="fields" hover striped class="mt-4">
      <template v-slot:cell(title)="data">
        {{ data.value | truncate(50) }}
      </template>

      <template v-slot:cell(posterUrl)="data">
        <img class="poster" :src="data.value" alt="" />
      </template>

      <template v-slot:cell(description)="data">
        {{ data.value | truncate(400) }}
      </template>

      <template v-slot:cell(genreIDs)="data">
        {{ data.value | toGenresLabel }}
      </template>

      <template v-slot:cell(edit)="data">
        <b-button :href="`/films/${data.item.id}`" variant="primary">
          <i class="fa-solid fa-pen"></i>
        </b-button>
      </template>

      <template v-slot:cell(remove)="data">
        <b-button v-on:click="removeFilm(data.item.id)" variant="danger">
          <i class="fa-solid fa-trash"></i>
        </b-button>
      </template>
    </b-table>
  </main>
</template>

<script lang="ts">
import Vue from 'vue';
import axios from 'axios';

import genres from '@/domain/genres';

export default Vue.extend({
  name: 'ListFilmsPage',
  data() {
    return {
      films: [],
      fields: [
        {
          key: 'id',
          label: 'ID',
        },
        {
          key: 'title',
          label: 'Назва',
        },
        {
          key: 'posterUrl',
          label: 'Постер',
        },
        {
          key: 'description',
          label: 'Опис',
        },
        {
          key: 'genreIDs',
          label: 'Жанри',
        },
        {
          key: 'edit',
          label: '',
        },
        {
          key: 'remove',
          label: '',
        },
      ],
    };
  },
  async mounted() {
    this.films = await this.getFilmsList();
  },
  filters: {
    toGenresLabel(genreIDs: number[]) {
      if (!genreIDs) {
        return null;
      }

      return genreIDs.map((genreID) => genres[genreID]).join(', ');
    },
  },
  methods: {
    async getFilmsList() {
      const response = await axios.get('/films/list');

      return response.data.films;
    },
    async refreshFilmsList() {
      this.films = await this.getFilmsList();
    },
    async removeFilm(filmID: number) {
      await axios.delete(`/films/${filmID}`);
      await this.refreshFilmsList();
    },
  },
});
</script>

<style scoped lang="scss">
.poster {
  max-height: 200px;
}
</style>
