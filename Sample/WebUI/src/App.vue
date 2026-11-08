<script setup lang="ts">
import { version } from '../package.json'
</script>

<template>
  <main class="hello">
    <h1>My Personal Finances</h1>
    <p class="muted">WebUI sample — v{{ version }}</p>
  </main>
</template>

<style scoped>
.hello {
  min-height: 100dvh;
  display: grid;
  place-content: center;
  text-align: center;
  gap: 0.5rem;
  /* Minimal stand-in tokens until M2 ships the theme registry */
  background: light-dark(#f6f7f9, #14161a);
  color: light-dark(#1c2128, #e7ebee);
}
.muted {
  opacity: 0.6;
  font-size: 0.9rem;
}
</style>
