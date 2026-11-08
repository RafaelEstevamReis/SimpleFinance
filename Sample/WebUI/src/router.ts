import { createRouter, createWebHistory } from 'vue-router'
import { getActive, getServerUrl } from './api/storage'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'server',
      component: () => import('./views/ServerView.vue'),
    },
    {
      path: '/connect',
      name: 'connect',
      component: () => import('./views/ConnectView.vue'),
    },
    {
      path: '/home',
      name: 'home',
      component: () => import('./views/HomeView.vue'),
    },
  ],
})

// Setup order is enforced by guards: server first, then an account, then home.
router.beforeEach((to) => {
  if (to.name !== 'server' && getServerUrl(localStorage) === null) {
    return { name: 'server' }
  }
  if (to.name === 'home' && getActive(localStorage) === null) {
    return { name: 'connect' }
  }
})

export default router
