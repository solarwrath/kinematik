<template>
  <main>
    <h1>Сеанси</h1>

    <div class="d-flex flex-row align-items-end">
      <b-form-group label="Фільм для створення нового сеансу:">
        <b-form-select v-model="filmToAdd" :options="filmsPool"></b-form-select>
      </b-form-group>

      <b-button v-on:click="addSession()" variant="success" class="ms-3" size="sm">
        <i class="fa-solid fa-plus"></i> Додати сеанс
      </b-button>
    </div>

    <div class="gstc-wrapper mt-5" ref="gstc"></div>

    <div class="d-flex flex-row justify-content-end mt-4">
      <b-button variant="success" v-on:click="saveSessions()">
        <i class="fa-solid fa-floppy-disk"></i> Зберегти
      </b-button>
    </div>
  </main>
</template>

<script lang="ts">
import Vue from 'vue';

import axios from 'axios';

import GSTC from 'gantt-schedule-timeline-calendar/dist/gstc.wasm.esm.min.js';
import { Item, Config, Rows, Items, PeriodString } from 'gantt-schedule-timeline-calendar';
import { Plugin as CalendarScroll } from 'gantt-schedule-timeline-calendar/dist/plugins/calendar-scroll.esm.min.js';
import { Plugin as TimelinePointer } from 'gantt-schedule-timeline-calendar/dist/plugins/timeline-pointer.esm.min.js';
import { Plugin as Selection } from 'gantt-schedule-timeline-calendar/dist/plugins/selection.esm.min.js';
import { Plugin as ItemMovement } from 'gantt-schedule-timeline-calendar/dist/plugins/item-movement.esm.min.js';
import 'gantt-schedule-timeline-calendar/dist/style.css';

import dayjs from 'dayjs';
import ukLocale from 'dayjs/locale/uk';

import tippy from 'tippy.js';
import 'tippy.js/dist/tippy.css';

import { v4 as uuidv4 } from 'uuid';

const UNASSIGNED_ROW_ID = 'UNASSIGNED';

let gstc, state;

function composeGanttRowsFromHalls(halls): Rows {
  if (!halls) {
    return {};
  }

  return halls.reduce((computedHalls, currentHall) => {
    const innerHallID = GSTC.api.GSTCID(currentHall.id.toString());
    computedHalls[innerHallID] = {
      id: innerHallID,
      label: currentHall.title,
    };

    return computedHalls;
  }, {});
}

function composeGanttItem(filmID, hallID, startAt, filmColors, filmRegistry, sessionID = null) {
  if (hallID === null) {
    hallID = UNASSIGNED_ROW_ID;
  }

  const itemID = GSTC.api.GSTCID(`${hallID}_${filmID}_${uuidv4()}`);
  const rowId = GSTC.api.GSTCID(hallID.toString());

  const film = filmRegistry[filmID];
  const filmRuntime = film.runtime;

  const startDate = GSTC.api.date(startAt, true);
  const endDate = startDate.add(filmRuntime, 'minute');
  const color = filmColors[filmID];

  return {
    id: itemID,
    label: film.title,
    rowId,
    time: {
      start: startDate.valueOf(),
      end: endDate.valueOf(),
    },
    filmID,
    color,
    sessionID,
  };
}

function composeGanntItems(sessions, filmColors, filmRegistry): Items {
  // return exampleSessionsData.reduce((generatedItems, currentSession) => {
  return sessions.reduce((generatedItems, currentSession) => {
    const composedItem = composeGanttItem(
      currentSession.filmID,
      currentSession.hallID,
      currentSession.startAt,
      filmColors,
      filmRegistry,
      currentSession.id
    );

    generatedItems[composedItem.id] = composedItem;

    return generatedItems;
  }, {});
}

function removeSessionItem(itemID: string) {
  state.update('config.chart.items', (items) => {
    delete items[itemID];
    return items;
  });
}

function isCollision(item) {
  const allItems = gstc.api.getAllItems();
  for (const itemId in allItems) {
    if (itemId === item.id) continue;
    const currentItem = allItems[itemId];
    if (currentItem.rowId === item.rowId) {
      if (item.time.start >= currentItem.time.start && item.time.start <= currentItem.time.end)
        return true;
      if (item.time.end >= currentItem.time.start && item.time.end <= currentItem.time.end)
        return true;
      if (item.time.start <= currentItem.time.start && item.time.end >= currentItem.time.end)
        return true;
      if (item.time.start >= currentItem.time.start && item.time.end <= currentItem.time.end)
        return true;
    }
  }
  return false;
}

function isItemUnassigned(item) {
  return GSTC.api.sourceID(item.rowId) === UNASSIGNED_ROW_ID;
}

const selectionPluginConfig = {
  cells: false,
  rectangularSelection: false,
  multipleSelection: false,
  showOverlay: false,
};

const days = [
  {
    zoomTo: 100, // we want to display this format for all zoom levels until 100
    period: 'day' as PeriodString,
    periodIncrement: 1,
    format({ timeStart }) {
      return timeStart.format('DD MMMM'); // full list of formats: https://day.js.org/docs/en/display/format
    },
  },
];

const hours = [
  {
    zoomTo: 100, // we want to display this format for all zoom levels until 100
    period: 'hour' as PeriodString,
    periodIncrement: 1,
    format({ timeStart }) {
      return timeStart.format('HH'); // full list of formats: https://day.js.org/docs/en/display/format
    },
  },
];

const minutes = [
  {
    zoomTo: 100, // we want to display this format for all zoom levels until 100
    period: 'minute' as PeriodString,
    periodIncrement: 15,
    main: true,
    format({ timeStart }) {
      return timeStart.format('mm');
    },
  },
];

function setTippyContent(element, data) {
  if (!gstc) return;
  if ((!data || !data.item) && element._tippy) return element._tippy.destroy();
  const itemData = gstc.api.getItemData(data.item.id);
  if (!itemData) {
    if (element._tippy) {
      return element._tippy.destroy();
    } else {
      return;
    }
  }
  if (itemData.detached && element._tippy) return element._tippy.destroy();
  // @ts-ignore
  if (!itemData.detached && !element._tippy) tippy(element, { trigger: 'mouseenter click' });
  if (!element._tippy) return;
  const startDate = itemData.time.startDate;
  const endDate = itemData.time.endDate;

  const startDateLabel = startDate.format('HH:mm');
  const endDateLabel = endDate.format('HH:mm');
  const tooltipContent = `"${data.item.label}" з ${startDateLabel} до ${endDateLabel}`;
  element._tippy.setContent(tooltipContent);
}

function itemTippy(element, data) {
  setTippyContent(element, data);
  return {
    update(element, data) {
      if (element._tippy) element._tippy.destroy();
      setTippyContent(element, data);
    },
    destroy(element, data) {
      if (element._tippy) element._tippy.destroy();
    },
  };
}

export default Vue.extend({
  name: 'MovieSessionsPage',
  data() {
    return {
      filmToAdd: null,
      filmsPool: [],
      filmColors: [],
      filmRegistry: [],
      periodStart: null,
    };
  },
  async mounted() {
    const films = await this.getFilmsAvailableForAssigningSessions();
    const filmRegistry = films.reduce((accumulatedFilmsRegistry, currentFilm) => {
      accumulatedFilmsRegistry[currentFilm.id] = {
        title: currentFilm.title,
        runtime: currentFilm.runtime,
        posterUrl: currentFilm.posterUrl,
      };

      return accumulatedFilmsRegistry;
    }, {});
    this.filmRegistry = filmRegistry;

    const filmIDs = Object.keys(filmRegistry);
    const filmsPool = filmIDs.map((filmID) => {
      return {
        value: +filmID,
        text: filmRegistry[+filmID].title,
      };
    });
    this.filmsPool = filmsPool;
    if (this.filmsPool && this.filmsPool.length !== 0) {
      this.filmToAdd = filmsPool[0].value;
    }

    const halls = await this.getHalls();
    const hallRows = composeGanttRowsFromHalls(halls);

    const unassignedRowID = GSTC.api.GSTCID(UNASSIGNED_ROW_ID);
    const ganntRows = {
      [unassignedRowID]: {
        id: unassignedRowID,
        label: 'Нерозподілені',
      },
      ...hallRows,
    };

    const amountOfFilms = filmIDs.length;
    const generatedColorPaletteAmount = amountOfFilms > 0 ? amountOfFilms : 1;
    const filmColors = filmIDs.reduce((generatedColorMappings, filmID, index) => {
      const generatedHue = (index * (360 / generatedColorPaletteAmount) + 45) % 360;
      const generatedColor = `hsl(${generatedHue}, 100%, 50%)`;
      generatedColorMappings[filmID] = generatedColor;

      return generatedColorMappings;
    }, {});
    this.filmColors = filmColors;

    const sessions = await this.getSessions();
    const ganntItems = composeGanntItems(sessions, filmColors, filmRegistry);

    const periodStart = GSTC.api.date(null, true).startOf('day');
    this.periodStart = periodStart;
    const periodEnd = periodStart.add(2, 'week');

    const movementPluginConfig = {
      events: {
        onMove({ items }) {
          return items.before.map((beforeMovementItem, index) => {
            const afterMovementItem = items.after[index];
            const myItem: Item = GSTC.api.merge({}, afterMovementItem);

            const shouldPreventCollisionAfterMoving =
              !isItemUnassigned(afterMovementItem) && isCollision(myItem);
            const shouldPreventMovingOutOfBounds = dayjs(myItem.time.start).isBefore(periodStart);
            const shouldPrevent =
              shouldPreventCollisionAfterMoving || shouldPreventMovingOutOfBounds;

            if (shouldPrevent) {
              myItem.time = { ...beforeMovementItem.time };
              myItem.rowId = beforeMovementItem.rowId;
            }

            return myItem;
          });
        },
      },
      snapToTime: {
        start({ startTime, time }) {
          const minute = startTime.minute();
          const nearestMinute = Math.round(minute / 15) * 15;

          let result = GSTC.api.date(startTime, true);
          result = result.minute(nearestMinute);
          result = result.second(0);
          result = result.millisecond(0);

          return result;
        },
      },
    };

    /*
    const REGULAR_ROW_HEIGHT = 40;
    const SCROLL_HEIGHT = 20;
    const innerHeight = REGULAR_ROW_HEIGHT * 5 + SCROLL_HEIGHT + 1;
    */

    const config: Config = {
      licenseKey:
        '====BEGIN LICENSE KEY====\nk+d7UlLw5Mfpcywclk2j/xGu3LxyUgpZ6MQwewHu6CSHfJoyKE99FzPKXCDj9Lazr8TUdtL0Oqj+1WTVCwSafepk79v7z77CRIcCgyysiPAv/T04ZFucN6kTMDunT4jQvtE0WWcYS/EpPoLISYFjwJV9cojMxW5zvCWDwLGMJ5Mt7WlJgmeshogLwyKdSokHekWcYkaUlZe/ODE/10mgJuHx6iZnTpUUoLSl5MEUFuqqaSCfdkyO9IIl8MKIbrtylXTbMX3HZiKp9QBFP1ItLeii95A4aofzZJsK5caNNRD6fGiSA3LIDWTs/iYXJ3aW6ChjM99oA908tNC0bg8/9Q==||U2FsdGVkX18BZUfwgnMnur9mZZiERaL6vEeG3P6IL+2IdiD/D+FIg0JmdTe4ve7BNYLYDWu9E5/odf0qV178xm32eVRmZN+EJjJdzj05wSk=\nNwBc37O5s2oox8jGNi31rHU6WVXxVBjZzHR6oS1zEAp98uxMoTLz/6Sn4B9MXcEwE491uF10Dsmu3oEA5VZSjZoXAixBqZ5MRy33AU755tTU1hGJVKEPZQ0/u3KTEaD3c6rHDki0bfOZC8GZDRy7iqET12d0EzCIuVdhFG8HlxnPuPsi7CFKMnTkS2+kDidYb7IkGDsKzN6RE2pn8azaJFcr6TuD+y24chX/KFTV6yjjmdbdFAszEBoGjx1gW5DZ3qTYEyiAVK3C369M3yAp4O7We24hI67BcAX4g1Rr+MdetprqE5MfmO5p6SRyrC4+KMkTn0IUKtSPj72vR+K+fA==\n====END LICENSE KEY====',
      plugins: [
        CalendarScroll(),
        TimelinePointer(),
        Selection(selectionPluginConfig),
        ItemMovement(movementPluginConfig),
      ],
      //innerHeight,
      list: {
        columns: {
          data: {
            [GSTC.api.GSTCID('hallLabel')]: {
              id: GSTC.api.GSTCID('hallLabel'),
              width: 200,
              data: 'label',
              header: {
                content: 'Зала',
              },
            },
          },
        },
        rows: ganntRows,
        toggle: {
          display: false,
        },
      },
      chart: {
        items: ganntItems,
        calendarLevels: [days, hours, minutes],
        time: {
          zoom: 15,
          leftGlobal: periodStart.valueOf(),
          from: periodStart.valueOf(),
          to: periodEnd.valueOf(),
        },
      },
      actions: {
        'chart-timeline-items-row-item': [itemTippy],
      },
      locale: ukLocale,
      utcMode: true,
      slots: {
        // item content slot that will show circle with letter next to item label
        'chart-timeline-items-row-item': {
          inner: [
            (vido: any, props: any) => {
              const { onChange, html } = vido;
              onChange((newProps) => {
                if (newProps && newProps.item) {
                  props = newProps;
                }
              });

              function onClick(ev) {
                ev.stopPropagation();
                ev.preventDefault();

                removeSessionItem(props.item.id);
              }

              return (content) => {
                if (!props || !props.item) return content;
                return html`<div
                  class="session-item"
                  style="background-color: ${props.item.color};"
                >
                  ${content}
                  <button @click="${onClick}" class="session-item-delete">
                    <i class="fa-solid fa-xmark"></i>
                  </button>
                </div>`;
              };
            },
          ],
        },
      },
    };

    state = GSTC.api.stateFromConfig(config);
    gstc = GSTC({
      element: this.$refs.gstc,
      state,
    });
  },
  beforeDestroy() {
    if (gstc) {
      gstc.destroy();
    }
  },
  methods: {
    async getFilmsAvailableForAssigningSessions() {
      const response = await axios.get('/films/available-for-session-assigning');
      return response.data.films;
    },
    async getHalls() {
      const response = await axios.get('/halls');
      return response.data.halls;
    },
    async getSessions() {
      const response = await axios.get('/sessions');
      return response.data.sessions;
    },
    addSession() {
      const newItem = composeGanttItem(
        this.filmToAdd,
        UNASSIGNED_ROW_ID,
        this.periodStart,
        this.filmColors,
        this.filmRegistry
      );

      state.update(`config.chart.items.${newItem.id}`, newItem);
    },
    saveSessions() {
      const updatedSessions = Object.values(gstc.api.getAllItems()).map((session: any) => {
        const sourceHallID = gstc.api.sourceID(session.rowId);
        const hallID = sourceHallID === UNASSIGNED_ROW_ID ? null : +sourceHallID;

        return {
          id: session.sessionID,
          filmID: session.filmID,
          hallID,
          startAt: new Date(session.time.start),
        };
      });

      const request = {
        sessions: updatedSessions,
      };

      axios.put('/sessions', request);
    },
  },
});
</script>

<style lang="scss">
.session-item {
  width: 100%;
  border-radius: inherit;
  position: relative;
  padding-right: 8px;
}

.session-item-delete {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  right: 8px;

  color: inherit !important;
  background: none !important;
  outline: none !important;
  border: none !important;
}

.gstc-component {
  margin: 0;
  padding: 0;
}
</style>
