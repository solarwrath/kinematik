<template>
  <Fragment v-if="formData">
    <b-form-group label="Назва:">
      <b-input v-model="formData.title"></b-input>
    </b-form-group>

    <b-form-group label="Постер:">
      <div class="d-flex flex-row">
        <b-form-file v-model="formData.poster" accept="image/*" placeholder=""></b-form-file>

        <b-button
          v-if="formData.posterUrl || formData.poster"
          v-on:click="clearPosterImage()"
          variant="danger"
        >
          <i class="fa-solid fa-trash"></i>
        </b-button>
      </div>

      <b-img v-if="formData.posterUrl" :src="formData.posterUrl" class="image-preview mt-2"></b-img>
    </b-form-group>

    <b-form-group label="Опис:">
      <b-textarea v-model="formData.description" min-rows="5" max-rows="10" trim></b-textarea>
    </b-form-group>

    <b-form-group label="Жанри:">
      <b-form-select v-model="formData.genreIDs" :options="genresPool" multiple></b-form-select>
    </b-form-group>

    <b-form-group label="Мова:">
      <b-form-select v-model="formData.languageID" :options="languagesPool"></b-form-select>
    </b-form-group>

    <b-form-group label="Тривалість:">
      <b-input-group>
        <b-input v-model="formData.runtime" type="number"></b-input>
        <template #append>
          <b-input-group-text>хв.</b-input-group-text>
        </template>
      </b-input-group>
    </b-form-group>

    <b-form-group label="IMDB ID:">
      <b-input v-model="formData.imdbID"></b-input>
    </b-form-group>

    <b-form-group label="URL трейлера:">
      <b-input v-model="formData.trailerUrl"></b-input>
    </b-form-group>

    <b-form-group label="Спеціальне зображення:">
      <div class="d-flex flex-row">
        <b-form-file v-model="formData.featuredImage" accept="image/*" placeholder=""></b-form-file>

        <b-button
          v-if="formData.featuredImageUrl || formData.featuredImage"
          v-on:click="clearFeaturedImage()"
          variant="danger"
        >
          <i class="fa-solid fa-trash"></i>
        </b-button>
      </div>

      <b-img
        v-if="formData.featuredImageUrl"
        :src="formData.featuredImageUrl"
        class="image-preview mt-2"
      ></b-img>
    </b-form-group>
  </Fragment>
</template>

<script lang="ts">
import Vue from 'vue';
import { Fragment } from 'vue-fragment';

import genres from '@/domain/genres';
import languages from '@/domain/languages';
import FilmFormData from '@/components/shared-forms/FilmForm/FilmFormData';

const genresPool = Object.getOwnPropertyNames(genres).map((genreID) => {
  return {
    value: +genreID,
    text: genres[+genreID],
  };
});
const languagesPool = Object.getOwnPropertyNames(languages).map((languageID) => {
  return {
    value: +languageID,
    text: languages[+languageID],
  };
});

export default Vue.extend({
  name: 'FilmForm',
  components: {
    Fragment,
  },
  props: {
    formData: Object as () => FilmFormData | null,
  },
  model: {
    prop: 'formData',
  },
  data() {
    return {
      genresPool,
      languagesPool,
    };
  },
  methods: {
    clearPosterImage() {
      this.formData.poster = null;
      this.formData.posterUrl = null;
    },
    clearFeaturedImage() {
      this.formData.featuredImage = null;
      this.formData.featuredImageUrl = null;
    },
  },
});
</script>

<style scoped lang="scss">
.image-preview {
  max-height: 300px;
}
</style>
