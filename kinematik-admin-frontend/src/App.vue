<template>
  <div id="app" class="app py-5 px-3">
    <nav class="sidebar py-2 px-3">
      <router-link to="/films" active-class="active-menu-item">Фільми</router-link>
      <router-link to="/halls" active-class="active-menu-item">Зали</router-link>
      <router-link to="/movie-sessions" active-class="active-menu-item">Сеанси</router-link>
    </nav>

    <div class="content container">
      <router-view />
    </div>
  </div>
</template>

<style scoped lang="scss">
.app {
  display: grid;
  grid-template-columns: 200px auto;
  grid-template-areas: 'sidebar content';
}

.sidebar {
  grid-area: sidebar;

  display: flex;
  flex-direction: column;
}

.content {
  grid-area: content;
}

.active-menu-item {
  color: black;
}
</style>
