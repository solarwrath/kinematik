<template>
  <main>
    <h1>Редагувати залу</h1>

    <form class="mt-5">
      <template v-if="currentHallFormData">
        <b-form-group label="ID:">
          <b-input disabled v-model="hallID"></b-input>
        </b-form-group>

        <HallForm v-model="currentHallFormData"></HallForm>
      </template>

      <div class="d-flex flex-row justify-content-end mt-4 gap-2">
        <b-button v-on:click="updateHall()" :disabled="!currentHallFormData" variant="success">
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

import HallForm from '@/components/shared-forms/HallForm/HallForm.vue';
import HallFormData, { SeatType } from '@/components/shared-forms/HallForm/HallFormData';

export default Vue.extend({
  name: 'EditHallPage',
  components: {
    HallForm,
  },
  data() {
    return {
      hallID: this.$route.params.id,
      initialHallFormData: null as HallFormData | null,
      currentHallFormData: null as HallFormData | null,
    };
  },
  mounted: async function () {
    const response = await axios.get(`/halls/${this.hallID}`);

    const mappedLayoutItems = response.data.layoutItems.map((rawLayoutItem) => {
      return {
        rowID: rawLayoutItem.rowID,
        columnID: rawLayoutItem.columnID,
        seatType: rawLayoutItem.seatTypeID as SeatType,
      };
    });
    const groupedLayoutRows = _.groupBy(mappedLayoutItems, (layoutItem) => layoutItem.rowID);

    this.initialHallFormData = {
      title: response.data.title,
      layoutRows: Object.values(groupedLayoutRows),
    };

    this.currentHallFormData = _.cloneDeep(this.initialHallFormData);
  },
  methods: {
    updateHall() {
      const layoutItems = this.currentHallFormData.layoutRows.flat().map((rawLayoutItem) => {
        return {
          rowID: rawLayoutItem.rowID,
          columnID: rawLayoutItem.columnID,
          seatTypeID: rawLayoutItem.seatType,
        };
      });

      axios.put(`/halls/${this.hallID}`, {
        title: this.currentHallFormData.title,
        layoutItems: layoutItems,
      });
    },
  },
});
</script>

<style scoped lang="scss"></style>
