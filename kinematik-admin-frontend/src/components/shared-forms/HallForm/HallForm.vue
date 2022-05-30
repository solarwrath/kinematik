<template>
  <Fragment v-if="localFormData">
    <b-form-group label="Назва:">
      <b-input v-model="localFormData.title"></b-input>
    </b-form-group>

    <b-form-group label="Кількість рядів:">
      <b-input type="number" v-model="rowCount" min="1"></b-input>
    </b-form-group>

    <b-form-group label="Кількість місць в найширшому ряду:">
      <b-input type="number" v-model="colCount" min="1"></b-input>
    </b-form-group>

    <b-form-group label="Розкладка:">
      <div class="hall-layout">
        <div
          v-for="layoutRow in localFormData.layoutRows"
          class="hall-layout-row"
          :key="layoutRow.id"
        >
          <div
            v-for="layoutItem in layoutRow"
            v-on:click="changeLayoutItemType(layoutItem)"
            class="hall-layout-item"
            :class="getHallItemTypeClass(layoutItem.typeID)"
            :style="`--hall-item-relative-width: ${widthRegistry[layoutItem.typeID]};`"
            role="button"
            :key="`${layoutItem.rowID}_${layoutItem.columnID}}`"
          >
            <template v-if="layoutItem.typeID === hallItemTypes.COMMON">
              <img svg-inline class="icon" src="@/assets/seat.svg" />
            </template>
            <template v-else-if="layoutItem.typeID === hallItemTypes.VIP">
              <img svg-inline class="icon" src="@/assets/seat.svg" />
            </template>
            <template v-else-if="layoutItem.typeID === hallItemTypes.COUCH">
              <img svg-inline class="icon" src="@/assets/couch.svg" />
            </template>
          </div>
        </div>
      </div>
    </b-form-group>
  </Fragment>
</template>

<script lang="ts">
import Vue from 'vue';
import { Fragment } from 'vue-fragment';

import { tap, cloneDeep } from 'lodash';

import HallFormData, {
  HallLayoutItem,
  HallLayoutItemType,
} from '@/components/shared-forms/HallForm/HallFormData';

function generateLayoutRows(rowCount: number, colCount: number): HallLayoutItem[][] {
  return Array(rowCount)
    .fill(null)
    .map((_, rowID) => {
      return generateLayoutColumns(colCount, rowID, 0);
    });
}

function generateLayoutColumns(colCount: number, rowID: number, startingColumnID: number) {
  return Array(colCount)
    .fill(null)
    .map((_, columnID) => {
      return {
        rowID: rowID,
        columnID: startingColumnID + columnID,
        typeID: HallLayoutItemType.EMPTY,
      };
    });
}

const DEFAULT_ROW_COUNT = 3;
const DEFAULT_COL_COUNT = 3;

export default Vue.extend({
  name: 'HallForm',
  components: {
    Fragment,
  },
  props: {
    formData: {
      type: Object as () => HallFormData | null,
    },
  },
  model: {
    prop: 'formData',
  },
  data: () => {
    return {
      rowCount: DEFAULT_ROW_COUNT,
      colCount: DEFAULT_COL_COUNT,

      widthRegistry: {
        [HallLayoutItemType.EMPTY]: 1,
        [HallLayoutItemType.COMMON]: 1,
        [HallLayoutItemType.VIP]: 1,
        [HallLayoutItemType.COUCH]: 2,
      },
      hallItemTypes: {
        COMMON: HallLayoutItemType.COMMON,
        VIP: HallLayoutItemType.VIP,
        COUCH: HallLayoutItemType.COUCH,
      },
    };
  },
  computed: {
    localFormData() {
      return this.formData ? this.formData : { title: '', layoutRows: [] };
    },
  },
  watch: {
    rowCount: function (newRowCount, oldRowCount) {
      const diffRowCount = Math.abs(newRowCount - oldRowCount);

      if (newRowCount > oldRowCount) {
        this.addLayoutRows(diffRowCount);
      } else {
        this.removeLayoutRows(diffRowCount);
      }
    },
    colCount: function (newColCount, oldColCount) {
      const diffColCount = Math.abs(newColCount - oldColCount);

      if (newColCount > oldColCount) {
        this.addLayoutColumns(diffColCount);
      } else {
        this.removeLayoutColumns(diffColCount);
      }
    },
  },
  mounted: function () {
    if (!this.formData.layoutRows || this.formData.layoutRows.length === 0) {
      this.$emit(
        'input',
        tap(cloneDeep(this.localFormData), (v) => {
          v.title = '';
          v.layoutRows = generateLayoutRows(DEFAULT_ROW_COUNT, DEFAULT_COL_COUNT);
        })
      );
    }
  },
  methods: {
    update(key, value) {
      this.$emit(
        'input',
        tap(cloneDeep(this.localFormData), (v) => Vue.set(v, key, value))
      );
    },
    refreshBinding() {
      this.$emit('input', cloneDeep(this.localFormData));
    },
    addLayoutRows(addedRowCount: number) {
      this.localFormData.layoutRows.push(...generateLayoutRows(addedRowCount, +this.colCount));
      this.refreshBinding();
    },
    removeLayoutRows(removedRowCount: number) {
      this.localFormData.layoutRows.splice(-removedRowCount);
      this.refreshBinding();
    },
    addLayoutColumns(addedColCount: number) {
      this.localFormData.layoutRows.forEach((layoutRow) => {
        layoutRow.push(...generateLayoutColumns(addedColCount, layoutRow.id, layoutRow.length));
      });
      this.refreshBinding();
    },
    removeLayoutColumns(removedColCount: number) {
      this.localFormData.layoutRows.forEach((layoutRow) => layoutRow.splice(-removedColCount));
      this.refreshBinding();
    },

    changeLayoutItemType(layoutItem: HallLayoutItem) {
      switch (layoutItem.typeID) {
        case HallLayoutItemType.EMPTY:
          layoutItem.typeID = HallLayoutItemType.COMMON;
          break;
        case HallLayoutItemType.COMMON:
          layoutItem.typeID = HallLayoutItemType.VIP;
          break;
        case HallLayoutItemType.VIP:
          layoutItem.typeID = HallLayoutItemType.COUCH;
          break;
        case HallLayoutItemType.COUCH:
          layoutItem.typeID = HallLayoutItemType.EMPTY;
          break;
      }
      this.refreshBinding();
    },
    isEmptyHallItem(itemType: HallLayoutItemType) {
      return itemType === HallLayoutItemType.EMPTY;
    },
    getHallItemTypeClass(itemType: HallLayoutItemType) {
      switch (itemType) {
        case HallLayoutItemType.COMMON:
          return 'hall-item-common';
        case HallLayoutItemType.VIP:
          return 'hall-item-vip';
        case HallLayoutItemType.COUCH:
          return 'hall-item-couch';
        case HallLayoutItemType.EMPTY:
        default:
          return 'hall-item-empty';
      }
    },
  },
});
</script>

<style scoped lang="scss">
.hall-layout {
  display: flex;
  flex-direction: column;
  flex-wrap: nowrap;
  justify-content: center;
  align-items: center;

  --default-layout-gap: 0.5rem;
  gap: var(--default-layout-gap);

  & .hall-layout-row {
    display: flex;
    flex-direction: row;
    flex-wrap: nowrap;

    gap: var(--default-layout-gap);

    & .hall-layout-item {
      --hall-item-base-width: 3em;
      width: calc(
        var(--hall-item-relative-width, 1) * var(--hall-item-base-width) +
          (var(--hall-item-relative-width) - 1) * var(--default-layout-gap)
      );
      height: 3em;

      &.hall-item-empty {
        border: 1px solid rgba(33, 33, 33, 0.2);
        border-radius: 4px;
      }
    }
  }
}
</style>
