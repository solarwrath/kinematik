<template>
  <main>
    <h1>Додати фільм до афіші</h1>

    <form class="mt-5">
      <FilmForm v-model="filmFormData"></FilmForm>

      <div class="d-flex flex-row justify-content-end mt-4 gap-2">
        <b-button variant="success" v-on:click="addFilm()">
          <i class="fa-solid fa-floppy-disk"></i> Зберегти
        </b-button>
        <b-button variant="outline-success" v-on:click="addFilm()">
          <i class="fa-solid fa-check"></i> Зберегти та додати ще
        </b-button>
      </div>
    </form>
  </main>
</template>

<script lang="ts">
import Vue from 'vue';
import axios from 'axios';

import FilmForm from '@/components/shared-forms/FilmForm/FilmForm.vue';

export default Vue.extend({
  name: 'CreateFilmPage',
  components: {
    FilmForm,
  },
  data() {
    return {
      filmFormData: {
        title: '',
        poster: null,
        description: '',
        genreIDs: [],
        languageID: 1,
        runtime: 0,
        imdbID: '',
        trailerUrl: '',
        featuredImage: null,
      },
    };
  },
  methods: {
    addFilm() {
      const requestData = new FormData();

      requestData.set('Title', this.filmFormData.title);
      requestData.set('Description', this.filmFormData.description);
      requestData.set('SerializedGenreIDs', JSON.stringify(this.filmFormData.genreIDs));
      requestData.set('SerializedLanguageID', JSON.stringify(this.filmFormData.languageID));
      requestData.set('SerializedRuntime', JSON.stringify(this.filmFormData.runtime));
      requestData.set('ImdbID', this.filmFormData.imdbID);
      requestData.set('TrailerUrl', this.filmFormData.trailerUrl);

      if (this.filmFormData.poster !== null) {
        const fileToUpload: File = this.filmFormData.poster;
        requestData.set('Poster', fileToUpload, fileToUpload.name);
      }

      if (this.filmFormData.featuredImage !== null) {
        const fileToUpload: File = this.filmFormData.featuredImage;
        requestData.set('FeaturedImage', fileToUpload, fileToUpload.name);
      }

      axios.post('/films', requestData);
    },
  },
});
</script>

<style scoped lang="scss"></style>
