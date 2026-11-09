// Router — setup order enforced by guards: server first, then an account,
// then the app. The shell layout is a parent route so connect/server screens
// render without the top bar/nav, and every post-connect screen inherits it.

import { createRouter, createWebHistory } from 'vue-router'
import { getActive, getServerUrl } from './api/storage'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: () => import('./components/AppShell.vue'),
      children: [
        {
          path: '',
          name: 'home',
          component: () => import('./views/HomeView.vue'),
        },
        {
          path: 'wallets',
          name: 'wallets',
          component: () => import('./views/WalletsView.vue'),
        },
        {
          path: 'book',
          name: 'book',
          component: () => import('./views/BookView.vue'),
        },
        {
          path: 'categories',
          name: 'categories',
          component: () => import('./views/CategoriesView.vue'),
        },
        {
          path: 'search',
          name: 'search',
          component: () => import('./views/SearchView.vue'),
        },
        {
          path: 'settings',
          name: 'settings',
          component: () => import('./views/SettingsView.vue'),
        },
      ],
    },
    {
      path: '/connect',
      name: 'connect',
      component: () => import('./views/ConnectView.vue'),
    },
    {
      path: '/server',
      name: 'server',
      component: () => import('./views/ServerView.vue'),
    },
  ],
})

// Setup order is enforced by guards: server first, then an account, then home.
router.beforeEach((to) => {
  if (to.name !== 'server' && getServerUrl(localStorage) === null) {
    return { name: 'server' }
  }
  const shellRoute = to.matched.some((r) => r.name === undefined && r.children !== undefined)
  const needsAccount = to.name === 'connect' ? false : shellRoute
  if (needsAccount && getActive(localStorage) === null) {
    return { name: 'connect' }
  }
  if (to.name === 'connect' && getActive(localStorage) !== null) {
    // "Add or create" keeps connect reachable; going there voluntarily is fine
  }
})

export default router
