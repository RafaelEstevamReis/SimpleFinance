<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { ApiClient, ApiError } from '../api/client'
import { getServerUrl, saveServerUrl } from '../api/storage'
import type { HelloResponse } from '../api/types'

// Server selection — single-server by design: point once, stay there.
// The URL is validated against the real API before it is saved.
const router = useRouter()

const savedUrl = ref(getServerUrl(localStorage))
const url = ref(savedUrl.value ?? '')
const checking = ref(false)
const error = ref('')
const hello = ref<HelloResponse | null>(null)

async function selectServer(): Promise<void> {
  error.value = ''
  hello.value = null
  const baseUrl = url.value.trim()
  if (baseUrl === '') { error.value = 'Give the server URL'; return }
  checking.value = true
  try {
    const probe = new ApiClient(baseUrl)
    hello.value = await probe.hello()
    saveServerUrl(localStorage, baseUrl)
    savedUrl.value = baseUrl
  } catch (e) {
    if (e instanceof ApiError) {
      switch (e.kind) {
        case 'unreachable':
          error.value = `Unreachable: ${e.message}`
          break
        case 'not-a-webapi':
        case 'not-found':
          error.value = 'This URL answered but is not a Simple.Finance WebApi'
          break
        default:
          error.value = e.message
      }
    } else {
      error.value = (e as Error).message
    }
  } finally {
    checking.value = false
  }
}

function proceed(): void {
  router.push({ name: 'connect' })
}
</script>

<template>
  <main class="server">
    <h1>My Personal Finances</h1>
    <p class="muted">Point the app at a Simple.Finance WebApi</p>

    <form class="row" @submit.prevent="selectServer">
      <input v-model="url" placeholder="http://localhost:5080"
             inputmode="url" autocomplete="off" :autofocus="url === ''" />
      <button class="primary" :disabled="checking">
        {{ checking ? 'Checking…' : 'Select' }}
      </button>
    </form>

    <p v-if="hello" class="ok">
      {{ hello.service }} v{{ hello.version }} — connected target saved
    </p>
    <p v-if="error" class="err">{{ error }}</p>

    <button v-if="savedUrl !== null"
            class="continue" @click="proceed">
      Continue →
    </button>
  </main>
</template>

<style scoped>
.server {
  max-width: 480px;
  margin: 0 auto;
  padding: 2rem 1rem 4rem;
  display: flex;
  flex-direction: column;
  gap: 0.9rem;
}
h1 { margin: 0; font-size: 1.4rem; }
.muted { opacity: 0.65; margin: 0; }
.row { display: flex; gap: 0.5rem; flex-wrap: wrap; }
input {
  flex: 2 1 220px;
  background: light-dark(#fff, #1c2128);
  color: inherit;
  border: 1px solid light-dark(#c4c9cf, #3a4148);
  border-radius: 6px;
  padding: 0.5rem 0.6rem;
}
button {
  font: inherit;
  border: 1px solid light-dark(#c4c9cf, #3a4148);
  border-radius: 6px;
  background: light-dark(#e9edf1, #262c33);
  color: inherit;
  padding: 0.5rem 0.9rem;
  cursor: pointer;
}
button.primary {
  background: light-dark(#2563eb, #3b82f6);
  border-color: transparent;
  color: #fff;
}
button.continue { align-self: flex-start; }
button:disabled { opacity: 0.55; }
.ok { color: light-dark(#15803d, #4ade80); margin: 0; }
.err { color: light-dark(#b91c1c, #f87171); margin: 0; }
</style>
