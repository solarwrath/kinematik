import Vue from 'vue';
import VueRouter, { RouteConfig } from 'vue-router';

import ListFilmsPage from '@/pages/films/ListFilmsPage.vue';
import CreateFilmPage from '@/pages/films/CreateFilmPage.vue';
import EditFilmPage from '@/pages/films/EditFilmPage.vue';

import ListHallsPage from '@/pages/halls/ListHallsPage.vue';
import CreateHallPage from '@/pages/halls/CreateHallPage.vue';
import EditHallPage from '@/pages/halls/EditHallPage.vue';

import SessionsPage from '@/pages/sessions/SessionsPage.vue';

import SessionsWithBookingStatsPage from '@/pages/bookings/SessionsWithBookingStatsPage.vue';

Vue.use(VueRouter);

const routes: Array<RouteConfig> = [
  {
    path: '/',
    redirect: (to) => {
      return {
        path: '/films',
      };
    },
  },
  {
    path: '/films',
    component: ListFilmsPage,
  },
  {
    path: '/films/create',
    component: CreateFilmPage,
  },
  {
    path: '/films/:id',
    component: EditFilmPage,
  },
  {
    path: '/halls',
    component: ListHallsPage,
  },
  {
    path: '/halls/create',
    component: CreateHallPage,
  },
  {
    path: '/halls/:id',
    component: EditHallPage,
  },
  {
    path: '/sessions',
    component: SessionsPage,
  },
  {
    path: '/bookings',
    component: SessionsWithBookingStatsPage,
  },
];

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes,
});

export default router;
