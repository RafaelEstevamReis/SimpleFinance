// Settings — real preferences (M3): theme, display mode and account type.
// The server is the truth for all three; localStorage keeps preview caches
// only (Architecture.md — Theming). Account type is display-only by design:
// it labels the profile in the top bar, it changes no mechanics.

<script setup lang="ts">
import { ref } from 'vue'
import { errorMessage, session, setPreference } from '../state/session'
import { accountTypeLabels, accountTypeNames, type AccountTypeName } from '../api/accountTypes'
import { modeNames, type ModeName } from '../styles/mode'
import { accentOf, themeNames, type ThemeName } from '../styles/themes'

const saving = ref('')
const settingsError = ref('')

async function pick(name: 'theme' | 'mode' | 'account-type', value: string): Promise<void> {
  settingsError.value = ''
  saving.value = name
  try {
    await setPreference(name, value)
  } catch (e) {
    settingsError.value = errorMessage(e)
  } finally {
    saving.value = ''
  }
}

function pickTheme(name: ThemeName): void { void pick('theme', name) }
function pickMode(name: ModeName): void { void pick('mode', name) }
function pickType(name: AccountTypeName): void { void pick('account-type', name) }
</script>

<template>
  <section class="settings">
    <h2>Appearance</h2>
    <p>Display mode — light, dark, or follow the device.</p>
    <div class="types">
      <button
        v-for="m in modeNames" :key="m"
        :class="{ on: session.state.value.mode === m }"
        :disabled="saving === 'mode'"
        @click="pickMode(m)"
      >{{ m === 'system' ? 'Follow device' : m === 'light' ? 'Light' : 'Dark' }}</button>
    </div>

    <div class="gap"></div>
    <h2>Theme</h2>
    <p>Identity tint — travels with the account on the server.</p>
    <div class="swatches">
      <button
        v-for="name in themeNames" :key="name"
        class="swatch" :class="{ active: session.state.value.theme === name }"
        :style="{ '--swatch': accentOf(name) }"
        :title="name"
        :disabled="saving === 'theme'"
        @click="pickTheme(name)"
      ><span class="chip"></span></button>
    </div>

    <div class="gap"></div>
    <h2>Account type</h2>
    <p>Display only — a label on the profile, it changes no mechanics.</p>
    <div class="types">
      <button
        v-for="t in accountTypeNames" :key="t"
        :class="{ on: session.state.value.accountType === t }"
        :disabled="saving === 'account-type'"
        @click="pickType(t)"
      >{{ accountTypeLabels[t] }}</button>
    </div>

    <p v-if="settingsError" class="err">{{ settingsError }}</p>
  </section>
</template>

<style scoped>
.settings {
  background: var(--surface); border: 1px solid var(--border);
  border-radius: 14px; padding: 1rem 1.1rem; margin-top: 0.9rem;
}
h2 { margin: 0 0 0.2rem; font-size: 0.72rem; letter-spacing: 0.12em; text-transform: uppercase; color: var(--muted); }
p { margin: 0 0 0.8rem; font-size: 0.78rem; color: var(--muted); }
.gap { height: 1.2rem; }
.types { display: flex; gap: 0.5rem; flex-wrap: wrap; }
.types button {
  border: 1px solid var(--border); border-radius: 999px;
  background: transparent; color: var(--fg);
  font: inherit; font-size: 0.8rem; padding: 0.4rem 0.85rem; cursor: pointer;
}
.types button.on { background: var(--accent-soft); border-color: var(--accent); color: var(--accent); }
.types button:disabled { opacity: 0.55; }
.swatches { display: flex; gap: 0.5rem; }
.swatch {
  width: 1.5rem; height: 1.5rem; border-radius: 50%;
  border: 2px solid var(--surface); outline: 1px solid var(--border);
  background: var(--swatch); cursor: pointer; padding: 0;
}
.swatch.active { outline: 2px solid var(--accent); }
.swatch:disabled { opacity: 0.55; }
.err { color: var(--error); margin-top: 0.8rem; }
</style>
