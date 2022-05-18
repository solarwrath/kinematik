import axios from 'axios';

import Vue from 'vue';

import { BootstrapVue, BootstrapVueIcons } from 'bootstrap-vue';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue/dist/bootstrap-vue.css';

import '@fortawesome/fontawesome-free/css/all.min.css';

import Fragment from 'vue-fragment';
// @ts-ignore
import VueTruncate from 'vue-truncate-filter';

import router from '@//router';

import App from '@/App.vue';
import apiBaseUrl from '@/api-base-url';

axios.defaults.baseURL = apiBaseUrl;

Vue.use(BootstrapVue);
Vue.use(BootstrapVueIcons);

Vue.use(Fragment.Plugin);
Vue.use(VueTruncate);

Vue.config.productionTip = false;

new Vue({
  router,
  render: (h) => h(App),
}).$mount('#app');
