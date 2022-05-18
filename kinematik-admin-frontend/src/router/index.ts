import Vue from 'vue';
import VueRouter, { RouteConfig } from 'vue-router';

import ListFilmsPage from '@/pages/ListFilmsPage.vue';
import CreateFilmPage from '@/pages/CreateFilmPage.vue';
import EditFilmPage from '@/pages/EditFilmPage.vue';

Vue.use(VueRouter);

const routes: Array<RouteConfig> = [
  {
    path: '/',
    component: ListFilmsPage,
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
];

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes,
});

export default router;
