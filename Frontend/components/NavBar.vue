<template>
  <div class="NavBar">
    <Icon name="mdi:home-circle" class="homeBtn" @click="goHome" v-if="!showHome"/>
    <slot />
    <div class="Profil" @click="navigateProfile()" v-if="!showProfile"></div>
  </div>
</template>

<style>
.NavBar {
  width: calc(100%-30pt);
  height: 50pt;
  align-content: center;
  padding-left: 15pt;
  padding-right: 15pt;
  background-color: rgb(35, 35, 45);
  z-index: 2;
  border-bottom: solid 3pt rgb(155, 255, 170);
  position: sticky;
  top: 0;
}

.Profil {
  position: absolute;
  top: 5pt;
  right: 25pt;
  width: 32pt;
  height: 32pt;
  border-radius: 20pt;
  border: solid 3pt rgb(155, 255, 170);
}

.Profil:hover {
  animation: hoverProfil 0.05s linear forwards;
}

.homeBtn {
  margin-top: 5pt;
  margin-left: 10pt;
  margin-right: 25pt;
  width: 32pt;
  height: 32pt;
  background-color: transparent;
  font-size: 23pt;
  border: solid 2pt transparent;
  color: inherit;
  font-family: inherit;
  font-weight: inherit;
  text-align: center;
  align-items: center;
  -webkit-touch-callout: none;
  -webkit-user-select: none;
  -khtml-user-select: none;
  -moz-user-select: none;
  user-select: none;
}

.homeBtn:hover {
  color: rgb(35, 35, 45);
  background-color: rgb(155, 255, 170);
  font-family: inherit;
  font-weight: inherit;
  border-radius: 20pt;
  border: solid 2pt rgb(155, 255, 170);
}

@keyframes hoverProfil {
  from {
    background-color: transparent;
  }

  to {
    background-color: rgb(155, 255, 170);
  }
}
</style>

<script setup>
const { showProfile, showHome } = defineProps(['showProfile', 'showHome'])
function navigateProfile() {
  window.open("/Profile", "_self")
}
function goHome() {
  navigateTo("/")
}
</script>