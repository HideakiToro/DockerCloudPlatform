<template>
  <div class="NavBar">
    <slot />
    <div class="Profil" @click="navigateProfile()"></div>
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

@keyframes hoverProfil {
    from {
        background-color: transparent;
    }

    to {
        background-color: rgb(155, 255, 170);
    }
}
</style>

<script>
export default {
  methods: {
    navigateProfile() {
      window.open("/Profile", "_self")
    }
  }
}
</script>