<template>
  <div class="NavBar">
    <slot />
    <div class="Profil" @click="navigateProfile()"></div>
  </div>
</template>
<script>
export default {
  methods: {
    navigateProfile() {
      window.open("/Profile", "_self")
    }
  }
}
</script>