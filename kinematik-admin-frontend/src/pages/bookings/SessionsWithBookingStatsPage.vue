<template>
  <main>
    <h1>Бронювання по сеансам</h1>

    <b-table :items="sessions" :fields="fields" hover striped class="mt-4">
      <template v-slot:cell(film)="data">
        <div class="d-flex flex-row gap-2">
          <img class="poster" :src="data.item.posterUrl" alt="" />
          {{ data.item.filmTitle | truncate(50) }}
        </div>
      </template>

      <template v-slot:cell(bookingStats)="data">
        {{ data.item.bookedSeatsQuantity }} / {{ data.item.hallCapacity }}
      </template>

      <template v-slot:cell(startAt)="data">
        {{ data.value | date }}
      </template>

      <template v-slot:cell(showBookingsButton)="data">
        <b-button v-on:click="toggleBookingsVisibility(data)" variant="primary">
          <template v-if="data.detailsShowing">
            <i class="fa-solid fa-eye-slash"></i> Сховати бронювання
          </template>
          <template v-if="!data.detailsShowing">
            <i class="fa-solid fa-eye"></i> Показати бронювання
          </template>
        </b-button>
      </template>

      <template v-slot:row-details="data">
        <template v-if="!data.item.layoutRows">Завантажується... </template>

        <template v-if="data.item.layoutRows">
          <div class="hall-layout">
            <div
              v-for="layoutRow in data.item.layoutRows"
              class="hall-layout-row"
              :key="layoutRow.id"
            >
              <div
                v-for="layoutItem in layoutRow"
                class="hall-layout-item"
                :class="getSeatClass(layoutItem)"
                :style="`--hall-item-relative-width: ${widthRegistry[layoutItem.seatType]};`"
                :key="`${layoutItem.rowID}_${layoutItem.columnID}`"
                :id="`${layoutItem.rowID}_${layoutItem.columnID}`"
              >
                <template v-if="layoutItem.seatType === seatTypes.COMMON">
                  <img svg-inline class="icon" src="@/assets/seat.svg" />
                </template>
                <template v-else-if="layoutItem.seatType === seatTypes.VIP">
                  <img svg-inline class="icon" src="@/assets/seat.svg" />
                </template>
                <template v-else-if="layoutItem.seatType === seatTypes.COUCH">
                  <img svg-inline class="icon" src="@/assets/couch.svg" />
                </template>

                <template v-if="shouldShowBookingTooltip(layoutItem)">
                  <i class="fa-solid fa-circle-info booked-details-icon"></i>

                  <b-tooltip
                    :target="`${layoutItem.rowID}_${layoutItem.columnID}`"
                    triggers="hover"
                    custom-class="booking-details-tooltip"
                  >
                    <div>
                      Номер замовлення: <strong>{{ layoutItem.bookingOrderID }}</strong>
                    </div>
                    <div>
                      E-mail бронювальника: <strong>{{ layoutItem.bookedClientEmail }}</strong>
                    </div>
                    <div>
                      Телефон бронювальника: <strong>{{ layoutItem.bookedClientPhone }}</strong>
                    </div>
                  </b-tooltip>
                </template>
              </div>
            </div>
          </div>
        </template>
      </template>
    </b-table>
  </main>
</template>

<script lang="ts">
import Vue from 'vue';

import axios from 'axios';
import _ from 'lodash';
import dayjs from 'dayjs';
import classNames from 'classnames';

import { SeatType } from '@/components/shared-forms/HallForm/HallFormData';

enum SeatAvailabilityStatus {
  FREE = 1,
  BOOKED = 2,
  PAYED_FOR = 3,
}

export default Vue.extend({
  name: 'SessionsWithBookingStatsPage',
  data() {
    return {
      sessions: [],
      fields: [
        {
          key: 'id',
          label: 'ID сеансу',
          class: 'text-center',
        },
        {
          key: 'film',
          label: 'Фільм',
        },
        {
          key: 'hallTitle',
          label: 'Зала',
          class: 'text-center',
        },
        {
          key: 'bookingStats',
          label: 'Статус бронювань',
          class: 'text-center',
        },
        {
          key: 'startAt',
          label: 'Час початку',
        },
        {
          label: '',
          key: 'showBookingsButton',
          class: 'text-center',
        },
      ],
      expandedSessions: [],

      widthRegistry: {
        [SeatType.EMPTY]: 1,
        [SeatType.COMMON]: 1,
        [SeatType.VIP]: 1,
        [SeatType.COUCH]: 2,
      },
      seatTypes: {
        COMMON: SeatType.COMMON,
        VIP: SeatType.VIP,
        COUCH: SeatType.COUCH,
      },
    };
  },
  filters: {
    date(serializedDate: string) {
      const formattedDate = dayjs(serializedDate).format('dddd, DD MMMM, hh:mm');
      return formattedDate[0].toLocaleUpperCase() + formattedDate.slice(1);
    },
  },
  async mounted() {
    const sessions = await this.getSessionsWithBookingsStatistics();
    sessions.forEach((session) => (session.layoutRows = []));
    this.sessions = sessions;
  },
  methods: {
    async getSessionsWithBookingsStatistics() {
      const response = await axios.get('/sessions/with-booking-stats');

      return response.data.sessions;
    },
    async getDetailedBookingStatuses(sessionID: number) {
      const response = await axios.get(`/bookings/${sessionID}/detailed`);

      return response.data.detailedBookingStatuses;
    },
    async toggleBookingsVisibility(data) {
      const willShowDetails = !data.detailsShowing;

      data.toggleDetails();

      if (willShowDetails) {
        const detailedBookingStatuses = await this.getDetailedBookingStatuses(data.item.id);
        console.log(JSON.stringify(detailedBookingStatuses, null, 2));

        const mappedBookingStatuses = detailedBookingStatuses.map((bookingStatus) => {
          return {
            rowID: bookingStatus.rowID,
            columnID: bookingStatus.columnID,
            seatType: bookingStatus.seatTypeID as SeatType,
            seatAvailabilityStatus:
              bookingStatus.seatAvailabilityStatusID as SeatAvailabilityStatus,
            bookingOrderID: bookingStatus.bookingOrderID,
            bookedClientEmail: bookingStatus.bookedClientEmail,
            bookedClientPhone: bookingStatus.bookedClientPhone,
          };
        });
        const groupedLayoutRows = _.groupBy(
          mappedBookingStatuses,
          (bookingStatus) => bookingStatus.rowID
        );

        data.item.layoutRows = Object.values(groupedLayoutRows);
      } else {
        data.item.layoutRows = [];
      }
    },
    getSeatClass(layoutItem) {
      const classes = [];

      switch (layoutItem.seatType) {
        case SeatType.EMPTY:
        default:
          classes.push('hall-item-empty');
          break;
        case SeatType.COMMON:
          classes.push('hall-item-common');
          break;
        case SeatType.VIP:
          classes.push('hall-item-vip');
          break;
        case SeatType.COUCH:
          classes.push('hall-item-couch');
          break;
      }

      switch (layoutItem.seatAvailabilityStatus) {
        case SeatAvailabilityStatus.FREE:
        default:
          classes.push('hall-item-free');
          break;
        case SeatAvailabilityStatus.BOOKED:
          classes.push('hall-item-booked');
          break;
        case SeatAvailabilityStatus.PAYED_FOR:
          classes.push('hall-item-payed-for');
          break;
      }

      return classNames(classes);
    },
    shouldShowBookingTooltip(layoutItem) {
      return [SeatAvailabilityStatus.BOOKED, SeatAvailabilityStatus.PAYED_FOR].includes(
        layoutItem.seatAvailabilityStatus
      );
    },
  },
});
</script>

<style scoped lang="scss">
.poster {
  max-height: 50px;
}

.hall-layout {
  display: flex;
  flex-direction: column;
  flex-wrap: nowrap;
  justify-content: center;
  align-items: center;

  --default-layout-gap: 1.5rem;
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

      position: relative;
      .booked-details-icon {
        color: var(--fill-color);
        position: absolute;
        top: -0.55em;
        right: 0;
        background-color: var(--bs-table-hover-bg);
        background-clip: padding-box;
      }

      & svg {
        fill: var(--fill-color);
      }

      &.hall-item-free {
        --fill-color: grey;
      }
      &.hall-item-booked {
        --fill-color: yellow;
      }
      &.hall-item-payed-for {
        --fill-color: green;
      }
    }
  }
}
</style>

<style>
.booking-details-tooltip .tooltip-inner {
  text-align: left;
  max-width: 400px;
}
</style>
