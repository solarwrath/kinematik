<template>
  <main>
    <h1>Редагувати фільм</h1>

    <form class="mt-5">
      <template v-if="currentFilmFormData">
        <b-form-group label="ID:">
          <b-input disabled v-model="filmID"></b-input>
        </b-form-group>

        <FilmForm v-model="currentFilmFormData"></FilmForm>
      </template>

      <div class="d-flex flex-row justify-content-end mt-4 gap-2">
        <b-button v-on:click="updateFilm()" :disabled="!currentFilmFormData" variant="success">
          <i class="fa-solid fa-floppy-disk"></i> Зберегти
        </b-button>
        <b-button href="/films" variant="danger">
          <i class="fa-solid fa-cancel"></i> Скасувати
        </b-button>
      </div>
    </form>
  </main>
</template>

<script lang="ts">
import Vue from 'vue';
import axios from 'axios';

import _ from 'lodash';

import FilmForm from '@/components/shared-forms/FilmForm/FilmForm.vue';
import FilmFormData from '@/components/shared-forms/FilmForm/FilmFormData';

export default Vue.extend({
  name: 'EditFilmPage',
  components: {
    FilmForm,
  },
  data() {
    return {
      filmID: this.$route.params.id,
      initialFormData: null as FilmFormData | null,
      currentFilmFormData: null as FilmFormData | null,
    };
  },
  mounted: async function () {
    const response = await axios.get(`/films/${this.filmID}`);

    const film = {
      title: response.data.title,
      posterUrl: response.data.posterUrl,
      description: response.data.description,
      genreIDs: response.data.genreIDs,
      runtime: response.data.runtime,
      imdbID: response.data.imdbID,
      trailerUrl: response.data.trailerUrl,
      featuredImageUrl: response.data.featuredImageUrl,
    };

    this.initialFormData = {
      ...film,
      language: 'ukrainian',
    };

    this.currentFilmFormData = _.cloneDeep(this.initialFormData);
  },
  methods: {
    updateFilm() {
      if (!this.currentFilmFormData) {
        return;
      }

      const requestData = new FormData();

      requestData.set('Title', this.currentFilmFormData.title);
      requestData.set('Description', this.currentFilmFormData.description);
      requestData.set('SerializedGenreIDs', JSON.stringify(this.currentFilmFormData.genreIDs));
      requestData.set('SerializedRuntime', JSON.stringify(this.currentFilmFormData.runtime));
      requestData.set('ImdbID', this.currentFilmFormData.imdbID);
      requestData.set('TrailerUrl', this.currentFilmFormData.trailerUrl);

      if (this.currentFilmFormData.poster) {
        const fileToUpload: File = this.currentFilmFormData.poster;
        requestData.set('Poster', fileToUpload, fileToUpload.name);
      }
      const wasPosterDeleted =
        !this.currentFilmFormData.posterUrl && this.initialFormData.posterUrl !== null;
      requestData.set('SerializedWasPosterDeleted', JSON.stringify(wasPosterDeleted));

      if (this.currentFilmFormData.featuredImage) {
        const fileToUpload: File = this.currentFilmFormData.featuredImage;
        requestData.set('FeaturedImage', fileToUpload, fileToUpload.name);
      }
      const wasFeaturedImageDeleted =
        !this.currentFilmFormData.featuredImageUrl &&
        this.initialFormData.featuredImageUrl !== null;
      requestData.set('SerializedWasFeaturedImageDeleted', JSON.stringify(wasFeaturedImageDeleted));

      axios.put(`/films/${this.filmID}`, requestData);
    },
  },
});
</script>

<style scoped lang="scss"></style>
